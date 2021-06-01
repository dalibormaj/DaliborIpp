using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.Application.Utility.Authorization
{
    public class CustomAuthorizationConditionsBuilder
    {
        private List<CustomAuthorizationConditions> _conditionsList;
        private CustomAuthorizationConditions _conditions;

        public CustomAuthorizationConditionsBuilder()
        {
            _conditionsList = new List<CustomAuthorizationConditions>();
            _conditions = new CustomAuthorizationConditions();
        }

        public CustomAuthorizationConditionsBuilder AllowFor(params Enum[] args)
        {
            _conditions.UserTypesToCheck.AddRange(args.OfType<UserTypeEnum>().ToList());
            _conditions.UserStatusesToCheck.AddRange(args.OfType<UserStatusEnum>().ToList());

            return this;
        }

        public CustomAuthorizationConditionsBuilder AllowFor(UserParameterTypeEnum arg, string value)
        {
            _conditions.UserParametersToCheck.Add(arg, value);

            return this;
        }

        public CustomAuthorizationConditionsBuilder AllowFor(UserDataTypeEnum arg, string value)
        {
            _conditions.UserAdditionalDataToCheck.Add(arg, value);

            return this;
        }

        public CustomAuthorizationConditionsBuilder AllowFor(Func<bool> specialCondition)
        {
            _conditions.SpecialConditions.Add(specialCondition);

            return this;
        }

        public CustomAuthorizationConditionsBuilder HasPermission(ProviderEnum providerId, SectionTypeEnum sectionTypeId)
        {
            _conditions.UserPermissionsToCheck.Add(providerId, sectionTypeId);

            return this;
        }

        public void BuildCondition()
        {
            _conditionsList.Add(_conditions);
            _conditions = new CustomAuthorizationConditions();
        }

        public List<CustomAuthorizationConditions> GetResults()
        {
            if (_conditionsList is null || !_conditionsList.Any())
                throw new IppicaException(ReturnCodeEnum.Unknown, "CustomAuthorizationConditionsBuilder - Please use BuildCondition before GetResults!");
            return _conditionsList;
        }
    }
}
