using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class Currency : BaseDomainModel
    {
        public CurrencyEnum? CurrencyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string IsoCode { get; set; }
        public byte? NumberOfDecimals { get; set; }
        public decimal? BalanceTolerance { get; set; }
    }
}
