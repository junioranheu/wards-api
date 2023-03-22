using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Wards.API.Filters;
using Wards.Application.UsesCases.Auths.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Input;

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
            services.Configure<ApiBehaviorOptions>(o =>
                o.InvalidModelStateResponseFactory = actionContext =>
                    {
                        return new BadRequestObjectResult(new
                        {
                            Code = StatusCodes.Status400BadRequest,
                            Request_Id = Guid.NewGuid(),
                            Messages = actionContext.ModelState.Values.
                                SelectMany(x => x.Errors).
                                Select(x => x.ErrorMessage)
                        });
                    }
            );

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<UsuarioInputValidator>();
            services.AddValidatorsFromAssemblyContaining<RegistrarInputValidator>();
        }

        private static void AddHealthCheck(IServiceCollection services)
        {
            services.AddHealthChecks();
        }
    }
}