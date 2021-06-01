using Newtonsoft.Json;
using System;

namespace Sks365.Ippica.Common.Utility
{
    public static class DateTimeExtensions
    {
        public static string ToMicrosoftDate(this DateTime date)
        {
            return JsonConvert.SerializeObject(date, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            }).Replace("\\", string.Empty).Replace("\"", string.Empty);
        }
    }
}
