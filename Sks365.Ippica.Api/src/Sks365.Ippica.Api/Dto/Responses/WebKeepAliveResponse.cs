using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class WebKeepAliveResponse : ResponseBase
    {
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
