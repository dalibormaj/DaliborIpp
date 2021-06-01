using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Utility.Authorization
{
    public class CustomAuthorizationConditions
    {
        public List<UserTypeEnum> UserTypesToCheck { get; } = new List<UserTypeEnum>();
        public List<UserStatusEnum> UserStatusesToCheck { get; } = new List<UserStatusEnum>();
        public Dictionary<UserParameterTypeEnum, string> UserParametersToCheck { get; } = new Dictionary<UserParameterTypeEnum, string>();
        public Dictionary<UserDataTypeEnum, string> UserAdditionalDataToCheck { get; } = new Dictionary<UserDataTypeEnum, string>();
        public List<Func<bool>> SpecialConditions { get; set; } = new List<Func<bool>>();
        public Dictionary<ProviderEnum, SectionTypeEnum> UserPermissionsToCheck { get; } = new Dictionary<ProviderEnum, SectionTypeEnum>();
    }
}
