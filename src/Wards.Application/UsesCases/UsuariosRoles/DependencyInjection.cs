using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole;
using Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole.Commands;
using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole;
using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries;

namespace Wards.Application.UsesCases.UsuariosRoles
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsuariosRolesApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarUsuarioRoleUseCase, CriarUsuarioRoleUseCase>();
            services.AddScoped<ICriarUsuarioRoleCommand, CriarUsuarioRoleCommand>();

            services.AddScoped<IListarUsuarioRoleUseCase, ListarUsuarioRoleUseCase>();
            services.AddScoped<IListarUsuarioRoleQuery, ListarUsuarioRoleQuery>();

            return services;
        }
    }
}
