using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Dto
{
    public class PsrScommessaDto
    {
        [JsonProperty("importo_scommessa")]
        public int ImportoScommessa { get; set; }
        [JsonProperty("unita")]
        public int Unita { get; set; }
        [JsonProperty("moltiplicatore")]
        public int Moltiplicatore { get; set; }
        [JsonProperty("gruppo")]
        public List<PsrScommessaGruppoDto> Gruppo { get; set; }
    }
}