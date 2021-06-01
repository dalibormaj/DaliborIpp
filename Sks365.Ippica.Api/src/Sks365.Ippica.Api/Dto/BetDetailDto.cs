using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Api.Dto
{
    public class BetDetailDto
    {
        [JsonProperty("race")]
        public int Race { get; set; }
        [JsonProperty("palinsesto")]
        public int Palinsesto { get; set; }
        [JsonProperty("avvenimento")]
        public int Avvenimento { get; set; }
        [JsonProperty("sigla")]
        public string Sigla { get; set; }
        [JsonProperty("course")]
        public string Course { get; set; }
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("rdate")]
        public DateTime RDate { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("market")]
        public string Market { get; set; }
        [JsonProperty("info_agg")]
        public string InfoAgg { get; set; }
        [JsonProperty("info_agg_des")]
        public string InfoAggDes { get; set; }
        [JsonProperty("horse")]
        public int Horse { get; set; }
        [JsonProperty("odd")]
        public decimal Odd { get; set; }
        [JsonProperty("system")]
        public string System { get; set; }
        [JsonProperty("num_combs")]
        public int NumCombs { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("des")]
        public string Des { get; set; }
    }
}
