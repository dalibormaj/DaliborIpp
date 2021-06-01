using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class WebBetRequest : BaseDomainModel
    {
        public long? BetRequestId { get; set; }
        public int? GroupCode { get; set; }
        public int? ShopCode { get; set; }
        public string Skin { get; set; }
        public string UserAccount { get; set; }
        public long? BetSettlementId { get; set; }
        public BetSettlementReasonEnum? BetSettlementReasonId { get; set; }
    }
}