using Newtonsoft.Json;

namespace Sks365.Ippica.Api.Dto.Responses.Base
{
    public abstract class ResponseBase
    {
        [JsonProperty("ret_code")]
        public virtual int ReturnCode { get; set; }

        [JsonProperty("description")]
        public virtual string Description { get; set; }

        [JsonProperty("timestamp")]
        public virtual string Timestamp { get; set; }
    }
}
