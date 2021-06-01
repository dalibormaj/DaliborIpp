using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Model
{
    public class Transaction : BaseDomainModel
    {
        public long? TransactionId { get; set; }
        //public int? UserId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? WithdrawableAmount { get; set; }
        public CurrencyEnum? CurrencyId { get; set; }
        public long? RefundedTransactionId { get; set; }
    }
}
