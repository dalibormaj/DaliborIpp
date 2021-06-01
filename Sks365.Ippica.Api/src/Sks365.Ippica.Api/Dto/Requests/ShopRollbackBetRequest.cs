using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto.Requests
{
    public class ShopRollbackBetRequest
    {
        [JsonProperty("shop_id")]
        public int ShopId { get; set; }
        [JsonProperty("terminal_id")]
        public int TerminalId { get; set; }
        [JsonProperty("operator_id")]
        public string OperatorId { get; set; }
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("session")]
        public string Session { get; set; }
        [JsonProperty("terminal_str")]
        public string TerminalStr { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
