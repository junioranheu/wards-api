using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Auths.Logar;
using Wards.Application.UsesCases.Auths.Registrar;

namespace Wards.Application.UsesCases.Auths
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthsApplication(this IServiceCollection services)
        {
            services.AddScoped<ILogarUseCase, LogarUseCase>();

            services.AddScoped<IRegistrarUseCase, RegistrarUseCase>();

            return services;
        }
    }
}
