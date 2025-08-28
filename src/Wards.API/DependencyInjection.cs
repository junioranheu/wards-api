using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.IO.Compression;
using System.Text.Json.Serialization;
using Wards.API.Extensions;
using Wards.API.Filters;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Domain.Consts;
using Wards.Infrastructure.Data;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
        {
            IWebHostEnvironment env = builder.Environment;

            AddKestrel(services);
            AddCompression(services);
            AddControllers(services, env);
            AddObservability(services);
            AddMisc(services);
            AddValidators(services);
            AddHealthCheck(services);

            return services;
        }

        private static void AddKestrel(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30);
            });
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

        private static void AddControllers(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddControllers(x =>
            {
                x.Filters.Add<RequestFilter>();
                x.Filters.Add<ErrorFilter>();
            }).
                AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    x.JsonSerializerOptions.WriteIndented = env.IsDevelopment();
                });
        }

        private static void AddObservability(IServiceCollection services)
        {
            /// Foi necessário instalar estas seguintes dependências:
            /// OpenTelemetry;
            /// OpenTelemetry.Extensions.Hosting;
            /// OpenTelemetry.Instrumentation.AspNetCore;
            services.AddOpenTelemetry().
                ConfigureResource(resource => resource.AddService(SistemaConst.NomeSistema)).
                WithTracing(tracing => tracing.
                    AddAspNetCoreInstrumentation()
                );
        }

        private static void AddMisc(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();

            services.AddUserRateLimiting();
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
                        Codigo = StatusCodes.Status400BadRequest,
                        Data = ObterDetalhesDataHora(),
                        Caminho = actionContext.HttpContext.Request.Path,
                        Mensagens = actionContext.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage),
                        IsErro = true
                    };

                    return new JsonResult(obj);
                };
            });
            #endregion

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            AssemblyScanner? validators = AssemblyScanner.FindValidatorsInAssemblyContaining<WardInputAltValidator>();
            validators.ForEach(x => services.AddValidatorsFromAssemblyContaining(x.ValidatorType));
        }

        private static void AddHealthCheck(IServiceCollection services)
        {
            services.AddHealthChecks().AddDbContextCheck<WardsContext>(); // Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore;
        }
    }
}
