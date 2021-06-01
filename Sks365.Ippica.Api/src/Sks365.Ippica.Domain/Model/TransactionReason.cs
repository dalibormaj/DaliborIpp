namespace Sks365.Ippica.Domain.Model
{
    public class TransactionReason : BaseDomainModel
    {
        public short? Id { get; set; }
        public string Description { get; set; }
        public TransactionReasonGroup Group { get; set; }
    }
}
