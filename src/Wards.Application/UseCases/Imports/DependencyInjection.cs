using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.Imports.CriarExemploUsuario;

namespace Wards.Application.UseCases.Imports
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