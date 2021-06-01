using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag.AspNetCore;
using Sks365.Ippica.Api.Extensions;
using Sks365.Ippica.Application.Services;
using Sks365.Ippica.Application.Utility.Authorization;
using Sks365.Ippica.Application.Utility.EmailSender;
using Sks365.Ippica.Application.Utility.OperationRecorder;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess.Extensions;
using Sks365.Payments.WebApi.Client;
using Sks365.SessionTracker.Client;
using System;

namespace Sks365.Ippica.Api
{
    /// <summary>
    ///
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the autofac container.
        /// </summary>
        /// <value>
        /// The autofac container.
        /// </value>
        public ILifetimeScope AutofacContainer { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the configuration root.
        /// </summary>
        /// <value>
        /// The configuration root.
        /// </value>
        public IConfigurationRoot ConfigurationRoot { get; private set; }

        /// <summary>
        /// Configures the specified application.
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="provider">The provider.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomMiddlewares();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Swagger
            if (env.IsDevelopment() || env.IsEnvironment("Dev"))
            {
                app.UseOpenApi();
                app.UseSwaggerUi3(
                    options =>
                    {
                        //build a swagger endpoint for each discovered API version
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerRoutes.Add(new SwaggerUi3Route($"v{description.GroupName}", $"/swagger/v{description.GroupName}/swagger.json"));
                        }
                    });
            }
        }

        /// <summary>
        /// Configures the services.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddControllers()
                    .AddNewtonsoftJson()
                    .ConfigureApiBehaviorOptions(x =>
                    {
                        x.InvalidModelStateResponseFactory = context =>
                        {
                            var errorMessage = GetErrorMessage(context);
                            throw new IppicaException(ReturnCodeEnum.BadRequest, errorMessage);
                        };
                    });

            services.AddRouting();
            services.AddDefaultPaymentsApiClients(new Uri(Configuration.GetAppSettings().IntegrationApiUrls.PaymentsApiUrl));
            services.AddSwagger();
        }

        /// <summary>
        /// Configures the services within autofac container
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddAppSettings(Configuration);
            builder.AddSessionTracker(Configuration);
            builder.AddOperationRecorder(Configuration);
            builder.AddValidators();
            builder.AddMappers();
            builder.AddServices();
            builder.AddCustomAuthorization();
            builder.AddEmailSender();
            builder.AddUnitOfWork();
        }

        /// <summary>
        /// Get low-level errors.These errors occur first and don't even go through the other validation mechanisms like FluentValidataions
        /// Example: Request expects int but user passes string, wrong Enum conversions etc. 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetErrorMessage(ActionContext context)
        {
            string errorMessage = string.Empty;
            foreach (var item in context.ModelState)
            {
                var key = item.Key;
                var rawValue = item.Value.RawValue;
                var errors = item.Value.Errors;
                if ((errors?.Count ?? 0) > 0)
                {
                    errorMessage = $"Field {key} contains an invalid value";
                    break;
                }
            }

            return errorMessage;
        }
    }
}