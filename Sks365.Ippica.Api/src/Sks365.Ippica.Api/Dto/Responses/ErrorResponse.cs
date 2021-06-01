using Newtonsoft.Json;
using Sks365.Ippica.Common.Utility;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class ErrorResponse
    {
        [JsonProperty("ret_code")]
        public ReturnCodeEnum ReturnCode { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
