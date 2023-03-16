using FluentValidation;
using FluentValidation.AspNetCore;
using Wards.API.Filters;
using Wards.Application.UsesCases.Usuarios.Shared.Input;

namespace Wards.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionAPI(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AddControllers(builder);
            AddMisc(builder);
            AddValidators(services);

            return services;
        }

        private static void AddControllers(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(o => o.Filters.Add<RequestFilter>());
            builder.Services.AddControllers(o => o.Filters.Add<ErrorFilter>());

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
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