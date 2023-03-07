using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Imports.CriarExemploUsuario;

namespace Wards.Application.UsesCases.Imports
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddImportsApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarExemploUsuarioUseCase, CriarExemploUsuarioUseCase>();

            return services;
        }
    }
}