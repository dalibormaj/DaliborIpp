using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class WebUserBalanceResponse : ResponseBase
    {
        [JsonProperty("balance")]
        public int Balance { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("bonus")]
        public Bonus Bonus { get; set; }
    }
}
