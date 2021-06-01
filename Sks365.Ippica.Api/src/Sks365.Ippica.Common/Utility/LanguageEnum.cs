using System.ComponentModel;

namespace Sks365.Ippica.Common.Utility
{
    public enum LanguageEnum : byte
    {
        [Description("it-IT")]
        Italian = 1,

        [Description("en-GB")]
        English = 2,

        [Description("de-DE")]
        German = 3
    }
}
