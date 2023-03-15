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
            builder.Services.AddControllers(o => o.Filters.Add<RequestFilter>());
            builder.Services.AddControllers(o => o.Filters.Add<ErrorFilter>());
            builder.Services.AddMemoryCache();

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            AddValidators(services);

            return services;
        }

        private static void AddValidators(IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<UsuarioInputValidator>();
        }
    }
}