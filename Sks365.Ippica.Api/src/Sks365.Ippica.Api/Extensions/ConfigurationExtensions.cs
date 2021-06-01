using Microsoft.Extensions.Configuration;
using Sks365.Ippica.Common.Config;
using Sks365.Ippica.Common.Config.Abstraction;

namespace Sks365.Ippica.Api.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IAppSettings GetAppSettings(this IConfiguration configuration)
        {
            IAppSettings appSettings = new AppSettings();
            configuration.GetSection(typeof(AppSettings).Name).Bind(appSettings);

            return appSettings;
        }
    }
}