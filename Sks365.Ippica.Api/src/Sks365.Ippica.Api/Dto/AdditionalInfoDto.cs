using Newtonsoft.Json;

namespace Sks365.Ippica.Api.Dto
{
    public class AdditionalInfoDto
    {
        [JsonProperty("field1")]
        public string Field1 { get; set; }
        [JsonProperty("field2")]
        public int Field2 { get; set; }
        [JsonProperty("fieldn")]
        public string Fieldn { get; set; }
    }
}
