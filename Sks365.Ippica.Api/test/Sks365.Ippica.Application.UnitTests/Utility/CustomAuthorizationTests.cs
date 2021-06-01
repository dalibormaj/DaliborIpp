using Microsoft.AspNetCore.Http;
using Moq;
using Sks365.Ippica.Application.Utility.Authorization;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.FakeData.Domain;
using System;
using Xunit;

namespace Sks365.Ippica.Application.UnitTests.Utility
{
    public class CustomAuthorizationTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<IServiceProvider> _serviceProvider;
        private Mock<IIsbetsUnitOfWork> _isBetsUnitOfWork;
        private Mock<IUserRepository> _userRepository;
        private ICustomAuthorization _customAuthorization;

        public CustomAuthorizationTests()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _serviceProvider = new Mock<IServiceProvider>();
            _isBetsUnitOfWork = new Mock<IIsbetsUnitOfWork>();
            _userRepository = new Mock<IUserRepository>();

            _serviceProvider.Setup(x => x.GetService(typeof(IIsbetsUnitOfWork))).Returns(_isBetsUnitOfWork.Object);
            _httpContextAccessor.SetupGet(x => x.HttpContext.RequestServices).Returns(_serviceProvider.Object);
            _isBetsUnitOfWork.SetupGet(x => x.UserRepository).Returns(_userRepository.Object);

            _customAuthorization = new CustomAuthorization(_httpContextAccessor.Object);
        }

        [Fact]
        public void Check_User_Enabled()
        {
            // Arrange
            var enabledUser = new FakeUser().FakeData
                .RuleFor(x => x.Status, UserStatusEnum.Enabled)
                .RuleFor(x => x.UserTypeId, UserTypeEnum.User)
                .Generate();
            var disabledUser = new FakeUser().FakeData
                .RuleFor(x => x.Status, UserStatusEnum.Disabled)
                .RuleFor(x => x.UserTypeId, UserTypeEnum.User)
                .Generate();

            // Act
            var builder = new CustomAuthorizationConditionsBuilder();
            builder
                .AllowFor(UserTypeEnum.User)
                .AllowFor(UserStatusEnum.Enabled)
                .BuildCondition();
            var exceptionEnabled = Record.Exception(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(enabledUser));

            // Assert
            Assert.Null(exceptionEnabled);
            Assert.Throws<ForbiddenException>(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(disabledUser));
        }

        [Fact]
        public void Check_Administrator_Enabled_And_PartiallyOpen()
        {
            // Arrange
            var userEnabled = new FakeUser().FakeData
                .RuleFor(x => x.Status, UserStatusEnum.Enabled)
                .RuleFor(x => x.UserTypeId, UserTypeEnum.Administrator)
                .Generate();
            var userPartiallyOpen = new FakeUser().FakeData
                .RuleFor(x => x.Status, UserStatusEnum.PartiallyOpen)
                .RuleFor(x => x.UserTypeId, UserTypeEnum.Administrator)
                .Generate();
            var userDeleted = new FakeUser().FakeData
                .RuleFor(x => x.Status, UserStatusEnum.Deleted)
                .RuleFor(x => x.UserTypeId, UserTypeEnum.Administrator)
                .Generate();

            // Act
            var builder = new CustomAuthorizationConditionsBuilder();
            builder
                .AllowFor(UserTypeEnum.Administrator)
                .AllowFor(UserStatusEnum.Enabled, UserStatusEnum.PartiallyOpen)
                .BuildCondition();
            var exceptionEnabled = Record.Exception(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(userEnabled));
            var exceptionPartiallyOpen = Record.Exception(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(userPartiallyOpen));

            // Assert
            Assert.Null(exceptionEnabled);
            Assert.Null(exceptionPartiallyOpen);
            Assert.Throws<ForbiddenException>(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(userDeleted));
        }

        [Theory]
        [InlineData(UserParameterTypeEnum.CdcUser)]
        [InlineData(UserParameterTypeEnum.CancelWithdrawal)]
        public void Check_CdcUser_Enabled(UserParameterTypeEnum userParameter)
        {
            // Arrange
            var user = new FakeUser().FakeData
                .RuleFor(x => x.UserTypeId, UserTypeEnum.User)
                .RuleFor(x => x.Status, UserStatusEnum.Enabled)
                .Generate();

            _userRepository.Setup(x => x.GetUserParameterValue(user.UserId.Value, userParameter)).Returns("1");

            // Act
            var builder = new CustomAuthorizationConditionsBuilder();
            builder
                .AllowFor(UserTypeEnum.User)
                .AllowFor(UserStatusEnum.Enabled)
                .AllowFor(UserParameterTypeEnum.CdcUser, "1")
                .BuildCondition();


            // Assert
            if (userParameter == UserParameterTypeEnum.CdcUser)
            {
                var exception = Record.Exception(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(user));
                Assert.Null(exception);
            }
            else
            {
                Assert.Throws<ForbiddenException>(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(user));
            }

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Check_Special_Condition(bool specialCondition)
        {
            // Arrange
            var user = new FakeUser().FakeData
                .RuleFor(x => x.UserTypeId, UserTypeEnum.User)
                .RuleFor(x => x.Status, UserStatusEnum.Enabled)
                .Generate();

            // Act
            var builder = new CustomAuthorizationConditionsBuilder();
            builder
                .AllowFor(UserTypeEnum.User)
                .AllowFor(() =>
                {
                    return specialCondition;
                })
                .BuildCondition();

            // Assert
            if (specialCondition)
            {
                var exception = Record.Exception(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(user));
                Assert.Null(exception);
            }
            else
            {
                Assert.Throws<ForbiddenException>(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(user));
            }
        }

        [Theory]
        [InlineData(UserTypeEnum.User)]
        [InlineData(UserTypeEnum.Administrator)]
        public void Check_First_Or_Second_Condition(UserTypeEnum validUserType)
        {
            // Arrange
            var user = new FakeUser().FakeData
                .RuleFor(x => x.UserTypeId, UserTypeEnum.Administrator)
                .RuleFor(x => x.Status, UserStatusEnum.Enabled)
                .Generate();

            _userRepository.Setup(x => x.GetUserParameterValue(user.UserId.Value, UserParameterTypeEnum.CancelWithdrawal)).Returns("1");

            // Act
            var builder = new CustomAuthorizationConditionsBuilder();
            builder
                .AllowFor(UserTypeEnum.Agent)
                .AllowFor(UserStatusEnum.BeValidated)
                .BuildCondition();
            builder
                .AllowFor(validUserType)
                .AllowFor(UserParameterTypeEnum.CancelWithdrawal, "1")
                .BuildCondition();


            // Assert
            if (validUserType == UserTypeEnum.Administrator)
            {
                var exception = Record.Exception(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(user));
                Assert.Null(exception);
            }
            else
            {
                Assert.Throws<ForbiddenException>(() => _customAuthorization.ApplyConditions(builder.GetResults()).Autorize(user));
            }
        }


    }
}
