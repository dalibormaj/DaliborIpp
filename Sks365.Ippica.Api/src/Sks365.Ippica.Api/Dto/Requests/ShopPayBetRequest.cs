using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto.Requests
{
    public class ShopPayBetRequest
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
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("winning_amount")]
        public int WinningAmount { get; set; }
        [JsonProperty("refundable_amount")]
        public int RefundableAmount { get; set; }
        [JsonProperty("tax_stake")]
        public int TaxStake { get; set; }
        [JsonProperty("tax_win")]
        public int TaxWin { get; set; }
        [JsonProperty("credit")]
        public bool Credit { get; set; }
        [JsonProperty("session")]
        public string Session { get; set; }
        [JsonProperty("terminal_str")]
        public string TerminalStr { get; set; }
        [JsonProperty("games")]
        public string Games { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
