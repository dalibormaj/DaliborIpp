using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto.Responses.Base;
using System;

namespace Sks365.Ippica.Api.Dto.Responses
{
    public class ShopReserveBetResponse : ResponseBase
    {
        [JsonProperty("transaction")]
        public long Transaction { get; set; }
    }
}

