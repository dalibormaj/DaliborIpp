using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Dto
{
    public class PsrScommessaGruppoDto
    {
        [JsonProperty("codice")]
        public int Codice { get; set; }
        [JsonProperty("evento")]
        public List<PsrScommessaEventoDto> Evento { get; set; }
    }
}