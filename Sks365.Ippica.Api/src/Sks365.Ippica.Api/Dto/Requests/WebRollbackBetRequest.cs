using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto.Requests
{
    public class WebRollbackBetRequest
    {
        [JsonProperty("group_code")]
        public int GroupCode { get; set; }
        [JsonProperty("shop_code")]
        public int ShopCode { get; set; }
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("session")]
        public string Session { get; set; }
        [JsonProperty("skin")]
        public string Skin { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
