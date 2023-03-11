using Wards.Application.UsesCases.UsuariosRoles.Shared.Output;

namespace Wards.Application.Services.Usuarios.ListarUsuarioRolesCache
{
    public interface IListarUsuarioRolesCacheService
    {
        Task<IEnumerable<UsuarioRoleOutput>?> Execute(string email);
    }
}