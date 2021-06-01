using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class WebReserveBetResponse : ResponseBase
    {
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
        [JsonProperty("id_bonus")]
        public string BonusIs { get; set; }
    }
}
