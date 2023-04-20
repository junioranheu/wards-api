using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.UsuariosRoles.CriarUsuarioRole;
using Wards.Application.UseCases.UsuariosRoles.CriarUsuarioRole.Commands;
using Wards.Application.UseCases.UsuariosRoles.ObterUsuarioRole;
using Wards.Application.UseCases.UsuariosRoles.ObterUsuarioRole.Queries;

namespace Wards.Application.UseCases.UsuariosRoles
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
