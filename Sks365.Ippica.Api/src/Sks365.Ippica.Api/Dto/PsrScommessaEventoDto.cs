using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Dto
{
    public class PsrScommessaEventoDto
    {
        [JsonProperty("codice")]
        public int Codice { get; set; }
        [JsonProperty("scommessa")]
        public int Scom { get; set; }
        [JsonProperty("sistema")]
        public int Sistema { get; set; }
        [JsonProperty("mappa")]
        public List<MappaDto> Mappa { get; set; }
    }
}