using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;
using System;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class ShopPlaceBetResponse : ResponseBase
    {
        [JsonProperty("transaction")]
        public string Transaction { get; set; }
        [JsonProperty("extra")]
        public AdditionalInfoDto AdditionalInfo { get; set; }
    }
}

