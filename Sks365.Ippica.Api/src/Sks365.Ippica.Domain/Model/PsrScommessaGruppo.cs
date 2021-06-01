using System.Collections.Generic;

namespace Sks365.Ippica.Domain.Model
{
    public class PsrScommessaGruppo : BaseDomainModel
    {
        public long? PsrScommessaGruppoId { get; set; }
        public long? PsrScommessaId { get; set; }
        public long? BetId { get; set; }
        public int? Codice { get; set; }
        public List<PsrScommessaEvento> Evento { get; set; } = new List<PsrScommessaEvento>();
    }
}
