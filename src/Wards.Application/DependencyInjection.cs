using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Logs;
using Wards.Application.UsesCases.Usuarios;

namespace Wards.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services)
        {
            services.AddLogsApplication();
            services.AddUsuariosApplication();

            return services;
        }
    }
}
