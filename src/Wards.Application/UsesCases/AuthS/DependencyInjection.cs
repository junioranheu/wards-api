using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Auths.Logar;

namespace Wards.Application.UsesCases.Auths
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthsApplication(this IServiceCollection services)
        {
            services.AddScoped<ILogarUseCase, LogarUseCase>();

            return services;
        }
    }
}
