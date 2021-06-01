using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class ShopBetRequest : BaseDomainModel
    {
        public long? BetRequestId { get; set; }
        public int? ShopId { get; set; }
        public int? TerminalId { get; set; }
        public string TerminalStr { get; set; }
        public string Loyalty { get; set; }
        public BetSettlementStatusEnum? BetSettlementStatusId { get; set; }
    }
}