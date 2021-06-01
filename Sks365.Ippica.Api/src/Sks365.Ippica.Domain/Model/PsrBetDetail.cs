using System.Collections.Generic;

namespace Sks365.Ippica.Domain.Model
{
    public class PsrBetDetail : BaseDomainModel
    {
        public long? PsrBetDetailsId { get; set; }
        public long? BetId { get; set; }
        public int? Fsc { get; set; }
        public int? Conc { get; set; }
        public int? Pvend { get; set; }
        public int? Terminale { get; set; }
        public int? TipoGiocata { get; set; }
        public string Sezione { get; set; }
        public decimal? GiocataBonus { get; set; }
        public int? Race { get; set; }
        public int? Palinsesto { get; set; }
        public int? Avvenimento { get; set; }
        public int? Concorso { get; set; }
        public int? Tipo { get; set; }
        public int? TipoConcorso { get; set; }
        public int? IdGiocata { get; set; }
        public int? TipoCaratura { get; set; }
        public int? NumCarature { get; set; }
        public List<PsrScommessa> Scommessa { get; set; }
    }
}
