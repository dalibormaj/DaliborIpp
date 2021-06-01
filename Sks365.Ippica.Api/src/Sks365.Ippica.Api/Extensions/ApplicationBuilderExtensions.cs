using Microsoft.AspNetCore.Builder;
using Sks365.Ippica.Api.Middleware;

namespace Sks365.Ippica.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
