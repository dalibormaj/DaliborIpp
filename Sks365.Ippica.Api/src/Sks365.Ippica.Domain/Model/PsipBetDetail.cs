using System.Collections.Generic;

namespace Sks365.Ippica.Domain.Model
{
    public class PsipBetDetail : BaseDomainModel
    {
        public long? PsipBetDetailsId { get; set; }
        public long? BetId { get; set; }
        public int? Fsc { get; set; }
        public int? Conc { get; set; }
        public int? Pvend { get; set; }
        public int? Terminale { get; set; }
        public int? TipoGiocata { get; set; }
        public string Sezione { get; set; }
        public decimal? GiocataBonus { get; set; }
        public int Race { get; set; }
        public int? Palinsesto { get; set; }
        public int? Avvenimento { get; set; }
        public int? IdGiocata { get; set; }
        public List<PsipScommessa> Scommessa { get; set; }
    }
}
