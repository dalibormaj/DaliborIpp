using System;

namespace Sks365.Ippica.Domain.Model
{
    [Serializable]
    public class FixBetDetail : BaseDomainModel
    {
        public long? FixBetDetailsId { get; set; }
        public long? BetId { get; set; }
        public int? Race { get; set; }
        public int? Palinsesto { get; set; }
        public int? Avvenimento { get; set; }
        public string Course { get; set; }
        public string Sigla { get; set; }
        public int? Number { get; set; }
        public DateTime? RDate { get; set; }
        public int? Code { get; set; }
        public string Market { get; set; }
        public string InfoAgg { get; set; }
        public string InfoAggDes { get; set; }
        public int? Horse { get; set; }
        public decimal? Odd { get; set; }
        public string Country { get; set; }
        public string Des { get; set; }
    }
}
