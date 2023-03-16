using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Wards.API.Filters;
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

            return services;
        }

        private static void AddCompression(WebApplicationBuilder builder)
        {
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            builder.Services.Configure<BrotliCompressionProviderOptions>(o =>
            {
                o.Level = CompressionLevel.Fastest;
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(o =>
            {
                o.Level = CompressionLevel.SmallestSize;
            });
        }

        private static void AddControllers(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(o => o.Filters.Add<RequestFilter>());
            builder.Services.AddControllers(o => o.Filters.Add<ErrorFilter>());

            builder.Services.AddControllers()
                .AddNewtonsoftJson(o =>
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
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<UsuarioInputValidator>();
        }
    }
}