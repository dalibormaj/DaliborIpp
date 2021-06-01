using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace Sks365.Ippica.Api.Extensions
{
    /// <summary>
    /// ServiceCollection Extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the swagger versioning.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddApiVersioning();
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "VVV"; //Major and minor version... see: https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings
                options.SubstituteApiVersionInUrl = true;
            });
            var provider = services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var versionDesc in provider.ApiVersionDescriptions)
            {
                services.AddOpenApiDocument(document =>
                {
                    //Versioning
                    document.DocumentName = $"v{versionDesc.GroupName}";
                    document.Title = "Sks365.Ippica.Api";
                    document.Version = versionDesc.ApiVersion.ToString();
                    document.ApiGroupNames = new[] { versionDesc.GroupName };
                });
            }
        }
    }
}