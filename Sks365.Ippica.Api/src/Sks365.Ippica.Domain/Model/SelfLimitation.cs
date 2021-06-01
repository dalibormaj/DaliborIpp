using Sks365.Ippica.Domain.Model.Enums;
using System;

namespace Sks365.Ippica.Domain.Model
{
    public class SelfLimitation : BaseDomainModel
    {
        public SelflimitationTypeEnum? SelflimitationTypeId { get; set; }
        public decimal? LimitAmount { get; set; }
        public int? LimitDays { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? RemainingAmount { get; set; }
        public string ErrorMessage { get; set; }
        public bool? Selflimited { get; set; }
    }
}
