using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class BetTransaction : BaseDomainModel
    {
        public long? BetTransactionId { get; set; }
        public BetTransactionTypeEnum? BetTransactionTypeId { get; set; }
        public long? BetRequestId { get; set; }
        public long? BetId { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyCode { get; set; }
        public long? TransactionId { get; set; }
        public long? PaymentOrderId { get; set; }
        public long? RefundBetTransactionId { get; set; }
    }
}
