using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Dto
{
    public class BetDto : IBetDto
    {
        [JsonProperty("header")]
        public HeaderDto Header { get; set; }
        [JsonProperty("detail")]
        public List<BetDetailDto> Details { get; set; }
    }
}
