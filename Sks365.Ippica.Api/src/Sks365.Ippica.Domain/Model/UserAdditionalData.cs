using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class UserAdditionalData : BaseDomainModel
    {
        public UserDataTypeEnum? UserDataTypeId { get; set; }
        public string Value { get; set; }
    }
}
