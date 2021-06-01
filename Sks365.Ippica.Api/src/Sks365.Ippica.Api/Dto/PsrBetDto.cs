using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Dto
{
    public class PsrBetDto : IBetDto
    {
        [JsonProperty("fsc")]
        public int Fsc { get; set; }
        [JsonProperty("conc")]
        public int Conc { get; set; }
        [JsonProperty("pvend")]
        public int Pvend { get; set; }
        [JsonProperty("terminale")]
        public int Terminale { get; set; }
        [JsonProperty("tipo_giocata")]
        public int TipoGiocata { get; set; }
        [JsonProperty("session")]
        public string Session { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("sezione")]
        public string Sezione { get; set; }
        [JsonProperty("giocata_bonus")]
        public decimal GiocataBonus { get; set; }
        [JsonProperty("race")]
        public int Race { get; set; }
        [JsonProperty("palinsesto")]
        public int Palinsesto { get; set; }
        [JsonProperty("concorso")]
        public int Concorso { get; set; }
        [JsonProperty("tipo")]
        public int Tipo { get; set; }
        [JsonProperty("tipo_concorso")]
        public int TipoConcorso { get; set; }
        [JsonProperty("id_giocata")]
        public int IdGiocata { get; set; }
        [JsonProperty("competenza")]
        public DateTime? Competenza { get; set; }
        [JsonProperty("emission")]
        public DateTime? Emission { get; set; }
        [JsonProperty("emission_utc")]
        public DateTime? EmissionUtc { get; set; }
        [JsonProperty("prezzo")]
        public int Prezzo { get; set; }
        [JsonProperty("tipo_caratura")]
        public int TipoCaratura { get; set; }
        [JsonProperty("num_carature")]
        public int NumCarature { get; set; }
        [JsonProperty("scommessa")]
        public List<PsrScommessaDto> Scommessa { get; set; }
    }
}
