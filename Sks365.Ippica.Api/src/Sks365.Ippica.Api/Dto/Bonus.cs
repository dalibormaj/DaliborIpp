using Newtonsoft.Json;

namespace Sks365.Ippica.Api.Dto
{
    public class Bonus
    {
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("balance")]
        public string Balance { get; set; }
        [JsonProperty("min")]
        public string Min { get; set; }
        [JsonProperty("Max")]
        public string Max { get; set; }
        [JsonProperty("total")]
        public string Total { get; set; }
        [JsonProperty("minodd")]
        public string MinOdd { get; set; }
    }
}
