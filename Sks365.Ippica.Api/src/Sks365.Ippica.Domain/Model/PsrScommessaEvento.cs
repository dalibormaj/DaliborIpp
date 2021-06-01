namespace Sks365.Ippica.Domain.Model
{
    public class PsrScommessaEvento : BaseDomainModel
    {
        public long? PsrScommessaEventoId { get; set; }
        public long? PsrScommessaGruppoId { get; set; }
        public long? BetId { get; set; }
        public int? Codice { get; set; }
        public int? Scom { get; set; }
        public int? Sistema { get; set; }
        public string Mappa { get; set; }
    }
}
