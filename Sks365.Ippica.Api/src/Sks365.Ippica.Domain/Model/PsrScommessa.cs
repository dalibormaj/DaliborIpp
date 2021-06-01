using System.Collections.Generic;

namespace Sks365.Ippica.Domain.Model
{
    public class PsrScommessa : BaseDomainModel
    {
        public long? PsrScommessaId { get; set; }
        public long? PsrBetDetailsId { get; set; }
        public long? BetId { get; set; }
        public decimal? ImportoScommessa { get; set; }
        public int? Unita { get; set; }
        public int? Moltiplicatore { get; set; }

        public List<PsrScommessaGruppo> Gruppo { get; set; } = new List<PsrScommessaGruppo>();
    }
}
