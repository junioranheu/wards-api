using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Usuarios.ListarUsuarioRolesCache;
using Wards.Application.Services.Usuarios.ObterUsuarioCache;

namespace Wards.Application.Services.Usuarios
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsuariosService(this IServiceCollection services)
        {
            services.AddScoped<IListarUsuarioRolesCacheService, ListarUsuarioRolesCacheService>();

            services.AddScoped<IObterUsuarioCacheService, ObterUsuarioCacheService>();

            return services;
        }
    }
}
