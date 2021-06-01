using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class PaymentOrder : BaseDomainModel
    {
        public long? PaymentOrderId { get; set; }
        public long? TransactionId { get; set; }
        //public long? RefundTransactionId { get; set; }
        public decimal? Amount { get; set; }
        public CurrencyEnum? CurrencyId { get; set; }
        public PaymentOrderStatusEnum? StatusId { get; set; }
    }
}
