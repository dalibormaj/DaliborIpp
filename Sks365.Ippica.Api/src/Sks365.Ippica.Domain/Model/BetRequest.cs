using Sks365.Ippica.Domain.Model.Enums;
using System;

namespace Sks365.Ippica.Domain.Model
{
    [Serializable]
    public class BetRequest : BaseDomainModel
    {
        public long? BetRequestId { get; set; }
        public WebBetRequest WebBetRequest { get; set; }
        public ShopBetRequest ShopBetRequest { get; set; }
        public BetRequestTypeEnum? BetRequestTypeId { get; set; }
        public int? UserId { get; set; }
        public string Session { get; set; }
        public string TicketId { get; set; }
        public string ExternalId { get; set; }
        public string Game { get; set; }
        public string Games { get; set; }
        public long? BetId { get; set; }
        public string Ip { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
