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
        public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services)
        {
            AddCompression(services);
            AddControllers(services);
            AddMisc(services);
            AddValidators(services);
            AddHealthCheck(services);

            return services;
        }

        private static void AddCompression(IServiceCollection services)
        {
            services.AddResponseCompression(x =>
            {
                x.EnableForHttps = true;
                x.Providers.Add<BrotliCompressionProvider>();
                x.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(x =>
            {
                x.Level = CompressionLevel.Optimal;
            });

            services.Configure<GzipCompressionProviderOptions>(x =>
            {
                x.Level = CompressionLevel.Optimal;
            });
        }

        private static void AddControllers(IServiceCollection services)
        {
            services.AddControllers(x =>
            {
                x.Filters.Add<RequestFilter>();
                x.Filters.Add<ErrorFilter>();
            }).
                AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                });
        }

        private static void AddMisc(IServiceCollection services)
        {
            services.AddMemoryCache();
        }

        private static void AddValidators(IServiceCollection services)
        {
            #region api_behavior_validator
            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                {
                    var obj = new
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Messages = actionContext.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage),
                    };

                    return new JsonResult(obj);
                };
            });
            #endregion

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            AssemblyScanner? validators = AssemblyScanner.FindValidatorsInAssemblyContaining<WardInputValidator>();
            validators.ForEach(x => services.AddValidatorsFromAssemblyContaining(x.ValidatorType));
        }

        private static void AddHealthCheck(IServiceCollection services)
        {
            services.AddHealthChecks().AddDbContextCheck<WardsContext>(); // Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore;
        }
    }
}