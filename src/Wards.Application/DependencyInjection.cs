using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Auths;
using Wards.Application.UsesCases.Logs;
using Wards.Application.UsesCases.Tokens;
using Wards.Application.UsesCases.Usuarios;
using Wards.Application.UsesCases.UsuariosRoles;

namespace Wards.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services)
        {
            services.AddAuthsApplication();
            services.AddTokensApplication();
            services.AddLogsApplication();

            services.AddUsuariosApplication();
            services.AddUsuariosRolesApplication();

            return services;
        }
    }
}
