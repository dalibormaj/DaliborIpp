using Microsoft.AspNetCore.Http;
using Moq;
using Sks365.Ippica.Application.Services;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.FakeData.Domain;
using Sks365.SessionTracker.Client;
using System;
using Xunit;

namespace Sks365.Ippica.Application.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IIsbetsUnitOfWork> _isBetsUnitOfWork;
        private readonly Mock<IMstUnitOfWork> _mstUnitOfWork;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IBetRepository> _betRepository;
        private readonly Mock<IWalletRepository> _walletRepository;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<ISessionTracker> _sessionTracker;
        private readonly Mock<IServiceProvider> _serviceProvider;

        public UserServiceTests()
        {
            _isBetsUnitOfWork = new Mock<IIsbetsUnitOfWork>();
            _mstUnitOfWork = new Mock<IMstUnitOfWork>();
            _userRepository = new Mock<IUserRepository>();
            _betRepository = new Mock<IBetRepository>();
            _walletRepository = new Mock<IWalletRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _sessionTracker = new Mock<ISessionTracker>();
            _serviceProvider = new Mock<IServiceProvider>();

            _serviceProvider.Setup(x => x.GetService(typeof(IIsbetsUnitOfWork))).Returns(_isBetsUnitOfWork.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IMstUnitOfWork))).Returns(_mstUnitOfWork.Object);
            _httpContextAccessor.SetupGet(x => x.HttpContext.RequestServices).Returns(_serviceProvider.Object);
            _isBetsUnitOfWork.SetupGet(x => x.UserRepository).Returns(_userRepository.Object);
            _isBetsUnitOfWork.SetupGet(x => x.WalletRepository).Returns(_walletRepository.Object);
            _mstUnitOfWork.SetupGet(x => x.BetRepository).Returns(_betRepository.Object);

            _userService = new UserService(_httpContextAccessor.Object, _sessionTracker.Object);
        }

        [Theory]
        [InlineData(1000000, BookmakerEnum.IT)]
        [InlineData(2000000, BookmakerEnum.IT_SHOP)]
        public void Get_User_Simple(int userId, BookmakerEnum bookmaker)
        {
            // Arrange
            var fakeUser = new FakeUser()
               .FakeData.RuleFor(x => x.UserId, userId)
               .RuleFor(x => x.BookmakerId, bookmaker)
               .RuleFor(x => x.AdditionalData, x => null)
               .Generate();

            _userRepository.Setup(x => x.GetUser(userId)).Returns(fakeUser);

            // Act
            var user = _userService.GetUser(userId);

            // Assert
            Assert.NotNull(user);
            Assert.Null(user.AdditionalData);
            Assert.Equal(userId, user.UserId);
            Assert.Equal(bookmaker, user.BookmakerId);
        }

        [Fact]
        public void Get_User_With_Additional_Data()
        {
            // Arrange
            var fakeUser = new FakeUser().FakeData.Generate();

            _userRepository.Setup(x => x.GetUser(It.IsAny<int>())).Returns(fakeUser);

            // Act
            var user = _userService.GetUser(1000, getUserDetails: true);

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(user.AdditionalData);
        }

        [Fact]
        public void Get_User_Additional_Data()
        {
            // Arrange
            var fakeData = new FakeUserAdditionalData().FakeData.Generate(3);
            _userRepository.Setup(x => x.GetUserAdditionalData(It.IsAny<int>())).Returns(fakeData);

            // Act
            var additionalData = _userService.GetUserAdditionalData(1);

            // Assert
            Assert.NotNull(additionalData);
            Assert.Equal(3, additionalData.Count);
        }

        [Theory]
        [InlineData(UserParameterTypeEnum.CdcUser)]
        [InlineData(UserParameterTypeEnum.CancelWithdrawal)]
        public void Get_User_Parameter_Value(UserParameterTypeEnum parameter)
        {
            // Arrange
            _userRepository.Setup(x => x.GetUserParameterValue(It.IsAny<int>(), It.IsAny<UserParameterTypeEnum>())).Returns("1");

            // Act
            var value = _userService.GetUserParameterValue(1, parameter);

            // Assert
            Assert.Equal("1", value);
        }

        [Fact]
        public void Get_User_By_TicketId()
        {
            // Arrange
            var fakeBet = new FakeBet().FakeData.Generate();
            var fakeUser = new FakeUser().FakeData
                .RuleFor(x => x.UserId, fakeBet.UserId)
                .Generate();

            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet);
            _userRepository.Setup(x => x.GetUser(It.IsAny<int>())).Returns(fakeUser);


            // Act
            var user = _userService.GetUserByTicketId(fakeBet.TicketId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(fakeBet.UserId, user.UserId);
        }

        [Fact]
        public void Get_Bonus_Wallet()
        {
            var userId = 1000000;
            // Arrange
            var fakeWallet = new FakeBonusWallet().FakeData
                .RuleFor(x => x.UserId, userId)
                .Generate();
            _walletRepository.Setup(x => x.GetBonusWallet(userId)).Returns(fakeWallet);

            // Act
            var bonusWallet = _userService.GetBonusWallet(userId);

            // Assert
            Assert.NotNull(bonusWallet);
        }
    }
}
