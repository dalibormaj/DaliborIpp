using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class WebIdentificationResponse : ResponseBase
    {
        [JsonProperty("user_account")]
        public string UserAccount { get; set; }

        [JsonProperty("skin")]
        public string Skin { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
