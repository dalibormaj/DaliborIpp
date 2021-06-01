using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Sks365.Ippica.Domain.Model.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CurrencyEnum : byte
    {
        [EnumMember(Value = "EUR")]
        EUR = 1,
        [EnumMember(Value = "USD")]
        USD = 2,
        [EnumMember(Value = "ALL")]
        ALL = 3,
        [EnumMember(Value = "TRY")]
        TRY = 4,
        [EnumMember(Value = "GBP")]
        GBP = 5,
        [EnumMember(Value = "SEK")]
        SEK = 6,
        [EnumMember(Value = "PLN")]
        PLN = 7,
        [EnumMember(Value = "CHF")]
        CHF = 8,
        [EnumMember(Value = "MKD")]
        MKD = 9,
        [EnumMember(Value = "RON")]
        RON = 10,
        [EnumMember(Value = "NGN")]
        NGN = 11,
        [EnumMember(Value = "ZAR")]
        ZAR = 12,
        [EnumMember(Value = "GHS")]
        GHS = 13,
        [EnumMember(Value = "MDL")]
        MDL = 14,
        [EnumMember(Value = "BRL")]
        BRL = 15,
        [EnumMember(Value = "RSD")]
        RSD = 16,
        [EnumMember(Value = "TND")]
        TND = 17
    }
}