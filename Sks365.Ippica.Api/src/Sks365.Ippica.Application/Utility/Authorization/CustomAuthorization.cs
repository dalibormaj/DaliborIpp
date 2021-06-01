using Microsoft.AspNetCore.Http;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.Application.Utility.Authorization
{

    public class CustomAuthorization : ICustomAuthorization
    {
        private readonly IServiceProvider _serviceProvider;
        private List<CustomAuthorizationConditions> _conditionsList;

        public CustomAuthorization(IHttpContextAccessor context)
        {
            _serviceProvider = context.HttpContext.RequestServices;
        }

        public ICustomAuthorization ApplyConditions(List<CustomAuthorizationConditions> conditionsList)
        {
            _conditionsList = conditionsList;
            return this;
        }

        public void Autorize(int userId)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                User user = unitOfWork.UserRepository.GetUser(userId);
                Autorize(user);
            }
        }

        public void Autorize(string userName, BookmakerEnum bookmakerId)
        {
            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                var user = unitOfWork.UserRepository.GetUser(userName, bookmakerId);
                Autorize(user);
            }
        }

        public void Autorize(User user)
        {
            if (user == null) throw new IppicaException(ReturnCodeEnum.UserNotFound);
            if (_conditionsList == null) throw new IppicaException(ReturnCodeEnum.Unknown, "CustomAuthorization - Please use ApplyConditions before Authorize!");

            var unitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            using (unitOfWork)
            {
                foreach (var conditions in _conditionsList)
                {
                    var valid =
                        ValidUserTypes(user, conditions) &&
                        ValidUserStatuses(user, conditions) &&
                        ValidUserPermissions(user, conditions, unitOfWork) &&
                        ValidUserParameters(user, conditions, unitOfWork) &&
                        ValidUserAdditionalData(user, conditions, unitOfWork) &&
                        ValidSpecialConditions(user, conditions);

                    if (valid)
                        return;
                }
            }

            throw new ForbiddenException(user.GetUserLanguage());
        }

        private bool ValidUserTypes(User user, CustomAuthorizationConditions conditions)
        {
            if (!conditions.UserTypesToCheck.Any())
                return true;

            if (conditions.UserTypesToCheck.Contains((UserTypeEnum)user.UserTypeId))
                return true;

            return false;
        }

        private bool ValidUserStatuses(User user, CustomAuthorizationConditions conditions)
        {
            if (!conditions.UserStatusesToCheck.Any())
                return true;

            if (conditions.UserStatusesToCheck.Contains((UserStatusEnum)user.Status))
                return true;

            return false;
        }

        private bool ValidUserPermissions(User user, CustomAuthorizationConditions conditions, IIsbetsUnitOfWork unitOfWork)
        {
            if (!conditions.UserPermissionsToCheck.Any())
                return true;

            foreach (var toCheck in conditions.UserPermissionsToCheck)
            {
                var hasPersmission = unitOfWork.UserRepository.GetUserAvailibility((int)user.UserId, (BookmakerEnum)user.BookmakerId, toCheck.Key, toCheck.Value);
                if (!hasPersmission)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidUserParameters(User user, CustomAuthorizationConditions conditions, IIsbetsUnitOfWork unitOfWork)
        {
            if (!conditions.UserParametersToCheck.Any())
                return true;

            foreach (var toCheck in conditions.UserParametersToCheck)
            {
                var paramValue = unitOfWork.UserRepository.GetUserParameterValue((int)user.UserId, toCheck.Key);
                if (toCheck.Value == paramValue)
                {
                    return true;
                };
            }

            return false;
        }

        private bool ValidUserAdditionalData(User user, CustomAuthorizationConditions conditions, IIsbetsUnitOfWork unitOfWork)
        {
            if (!conditions.UserAdditionalDataToCheck.Any())
                return true;


            user.AdditionalData = user.AdditionalData ?? unitOfWork.UserRepository.GetUserAdditionalData((int)user.UserId);

            foreach (var toCheck in conditions.UserAdditionalDataToCheck)
            {
                if (user.AdditionalData?.Find(x => x.UserDataTypeId == toCheck.Key && x.Value == toCheck.Value) != null)
                {
                    return true;
                };
            }

            return false;
        }

        private bool ValidSpecialConditions(User user, CustomAuthorizationConditions conditions)
        {
            foreach (var specCond in conditions.SpecialConditions)
            {
                var valid = specCond.Invoke();
                if (!valid)
                    return false;
            }

            return true;
        }

    }
}
