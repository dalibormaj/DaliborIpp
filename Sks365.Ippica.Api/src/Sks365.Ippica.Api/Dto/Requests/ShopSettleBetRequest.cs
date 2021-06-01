using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto.Requests
{
    public class ShopSettleBetRequest
    {
        [JsonProperty("tickets")]
        public TicketDto[] Tickets { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
