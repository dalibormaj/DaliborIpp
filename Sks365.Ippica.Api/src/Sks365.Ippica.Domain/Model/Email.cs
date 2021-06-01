using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class Email
    {
        public long? LogEmailId { get; set; }
        public BetRequestTypeEnum? BetRequestTypeId { get; set; }
        public long? BetId { get; set; }
        public string TicketId { get; set; }
        public string ExternalId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
