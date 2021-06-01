using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto
{
    public class HeaderDto
    {
        [JsonProperty("id_ticket")]
        public string TicketId { get; set; }
        [JsonProperty("emission")]
        public DateTime? Emission { get; set; }
        [JsonProperty("emission_utc")]
        public DateTime? EmissionUtc { get; set; }
        [JsonProperty("stake")]
        public int Stake { get; set; }
        [JsonProperty("max_winning")]
        public int MaxWinning { get; set; }
        [JsonProperty("bets")]
        public int Bets { get; set; }
        [JsonProperty("competence")]
        public DateTime? Competence { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("source")]
        public int Source { get; set; }
        [JsonProperty("antepost")]
        public int Antepost { get; set; }
    }
}
