using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sks365.Ippica.Domain.Model.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BookmakerEnum : short
    {
        COM = 5,
        IT = 7,
        IT_SHOP = 9,
        TN = 12,
        AT = 19,
        GER = 20
    }
}
