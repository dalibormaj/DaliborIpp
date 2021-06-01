using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto.Requests
{
    public class ShopPlaceBetRequest<TBet> where TBet : IBetDto
    {
        [JsonProperty("shop_id")]
        public int ShopId { get; set; }
        [JsonProperty("terminal_id")]
        public int TerminalId { get; set; }
        [JsonProperty("operator_id")]
        public string OperatorId { get; set; }
        [JsonProperty("session")]
        public string Session { get; set; }
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("tax_stake")]
        public int TaxStake { get; set; }
        [JsonProperty("tax_win")]
        public int TaxWin { get; set; }
        [JsonProperty("bet")]
        public string Bet { get; set; }
        [JsonProperty("loyalty")]
        public string Loyalty { get; set; }
        [JsonProperty("jbet")]
        public TBet JBet { get; set; }
        [JsonProperty("terminal_str")]
        public string TerminalStr { get; set; }
        [JsonProperty("games")]
        public string Games { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}