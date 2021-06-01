using System;

namespace Sks365.Ippica.Domain.Model
{
    [Serializable]
    public class LogRequest : BaseDomainModel
    {
        public string Endpoint { get; set; }
        public string HttpMethod { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int HttpStatusCode { get; set; }
        public string Session { get; set; }
        public string TicketId { get; set; }
        public string ExternalId { get; set; }
        public string UserAccount { get; set; }
        public string RemoteIpAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ResponseDate { get; set; }
    }
}
