using Newtonsoft.Json;

namespace Sks365.Ippica.Api.Dto
{
    public class TicketDto
    {
        [JsonProperty("shop_id")]
        public int ShopId { get; set; }
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("win_amount")]
        public int WinAmount { get; set; }
        [JsonProperty("refund_amount")]
        public int RefundAmount { get; set; }
        [JsonProperty("bet")]
        public string Bet { get; set; }
    }
}
