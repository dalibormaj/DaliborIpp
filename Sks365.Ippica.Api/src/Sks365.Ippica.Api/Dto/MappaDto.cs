using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Dto
{
    public class MappaDto
    {
        [JsonProperty("esito")]
        public List<int> Esito { get; set; }
    }
}