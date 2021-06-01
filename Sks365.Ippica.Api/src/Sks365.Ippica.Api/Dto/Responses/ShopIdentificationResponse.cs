using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;
using Sks365.Ippica.Common.Utility;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class ShopIdentificationResponse : ResponseBase
    {
        [JsonProperty("type")]
        public TerminalTypeEnum Type { get; set; }
        [JsonProperty("mst_code")]
        public int MstCode { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}

