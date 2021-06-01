using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class WebPlaceBetResponse : ResponseBase
    {
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
        [JsonProperty("id_bonus")]
        public string BonusId { get; set; }
    }
}
