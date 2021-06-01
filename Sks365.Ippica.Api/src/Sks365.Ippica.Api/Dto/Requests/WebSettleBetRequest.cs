using Newtonsoft.Json;
using Sks365.Ippica.Domain.Model.Enums;
using System;

namespace Sks365.Ippica.Api.Dto.Requests
{
    public class WebSettleBetRequest
    {
        [JsonProperty("group_code")]
        public int GroupCode { get; set; }
        [JsonProperty("shop_code")]
        public int ShopCode { get; set; }
        [JsonProperty("user_account")]
        public string UserAccount { get; set; }
        [JsonProperty("ticket_id")]
        public string TicketId { get; set; }
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("bonus")]
        public string Bonus { get; set; }
        [JsonProperty("id_bonus")]
        public string BonusId { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("total_amount")]
        public int TotalAmount { get; set; }
        [JsonProperty("payment_amount")]
        public int PaymentAmount { get; set; }
        [JsonProperty("refund_amount")]
        public int RefundAmount { get; set; }
        [JsonProperty("tax_stake")]
        public int TaxStake { get; set; }
        [JsonProperty("tax_win")]
        public int TaxWin { get; set; }
        [JsonProperty("reason")]
        public BetSettlementReasonEnum Reason { get; set; }
        [JsonProperty("skin")]
        public string Skin { get; set; }
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
        [JsonProperty("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
