using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Wards.API.Filters;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Infrastructure.Data;

namespace Wards.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AddCompression(builder);
            AddControllers(builder);
            AddMisc(builder);
            AddValidators(services);
            AddHealthCheck(services);

            return services;
        }

        private static void AddCompression(WebApplicationBuilder builder)
        {
            builder.Services.AddResponseCompression(o =>
            {
                o.EnableForHttps = true;
                o.Providers.Add<BrotliCompressionProvider>();
                o.Providers.Add<GzipCompressionProvider>();
            });

            builder.Services.Configure<BrotliCompressionProviderOptions>(o =>
            {
                o.Level = CompressionLevel.Optimal;
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(o =>
            {
                o.Level = CompressionLevel.Optimal;
            });
        }

        private static void AddControllers(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(o =>
            {
                o.Filters.Add<RequestFilter>();
                o.Filters.Add<ErrorFilter>();
            }).
                AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    o.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                });
        }

        private static void AddMisc(WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
        }

        private static void AddValidators(IServiceCollection services)
        {
            #region api_behavior_validator
            services.Configure<ApiBehaviorOptions>(o =>
                o.InvalidModelStateResponseFactory = actionContext =>
                    {
                        return new BadRequestObjectResult(new
                        {
                            Code = StatusCodes.Status400BadRequest,
                            Messages = actionContext.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
                        });
                    }
            );
            #endregion

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            AssemblyScanner? validators = AssemblyScanner.FindValidatorsInAssemblyContaining<WardInputValidator>();
            validators.ForEach(x => services.AddValidatorsFromAssemblyContaining(x.ValidatorType));
        }

        private static void AddHealthCheck(IServiceCollection services)
        {
            services.AddHealthChecks().AddDbContextCheck<WardsContext>(); // Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
        }
    }
}