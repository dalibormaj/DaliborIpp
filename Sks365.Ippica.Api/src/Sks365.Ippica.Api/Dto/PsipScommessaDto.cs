using System.Collections.Generic;

namespace Sks365.Ippica.Api.Dto
{
    public class PsipScommessaDto
    {
        public int Codice { get; set; }
        public int Importo { get; set; }
        public int Sistema { get; set; }
        public int Moltiplicatore { get; set; }
        public List<MappaDto> Mappa { get; set; }

    }
}