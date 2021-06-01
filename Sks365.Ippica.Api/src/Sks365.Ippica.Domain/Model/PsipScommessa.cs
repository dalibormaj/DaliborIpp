namespace Sks365.Ippica.Domain.Model
{
    public class PsipScommessa : BaseDomainModel
    {
        public long? PsipScommessaId { get; set; }
        public long? PsipBetDetailsId { get; set; }
        public long? BetId { get; set; }
        public int? Codice { get; set; }
        public decimal? Importo { get; set; }
        public int? Sistema { get; set; }
        public int? Moltiplicatore { get; set; }
        public string Mappa { get; set; }
    }
}
