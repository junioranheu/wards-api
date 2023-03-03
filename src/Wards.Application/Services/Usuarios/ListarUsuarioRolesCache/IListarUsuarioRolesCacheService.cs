using Wards.Domain.Entities;

namespace Wards.Application.Services.Usuarios.ListarUsuarioRolesCache
{
    public interface IListarUsuarioRolesCacheService
    {
        Task<IEnumerable<UsuarioRole>?> ListarUsuarioRolesCache(string email);
    }
}