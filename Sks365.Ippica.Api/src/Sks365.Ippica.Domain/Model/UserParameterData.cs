using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class UserParameterData : BaseDomainModel
    {
        public UserParameterTypeEnum? UserParameterTypeId { get; set; }
        public string Value { get; set; }
    }
}
