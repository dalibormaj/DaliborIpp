using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto.Requests
{
    public class WebReserveBetRequest<TBet> where TBet : IBetDto
    {
        [JsonProperty("group_code")]
        public int GroupCode { get; set; }
        [JsonProperty("shop_code")]
        public int ShopCode { get; set; }
        [JsonProperty("session")]
        public string Session { get; set; }
        [JsonProperty("skin")]
        public string Skin { get; set; }
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }
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
        [JsonProperty("bonus")]
        public int Bonus { get; set; }
        [JsonProperty("id_bonus")]
        public string BonusId { get; set; }
        [JsonProperty("bet")]
        public TBet Bet { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
