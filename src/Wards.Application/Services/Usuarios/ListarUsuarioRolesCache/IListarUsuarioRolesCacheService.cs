using Wards.Domain.Entities;

namespace Wards.Application.Services.Usuarios.ListarUsuarioRolesCache
{
    public interface IListarUsuarioRolesCacheService
    {
        Task<IEnumerable<UsuarioRole>?> Execute(string email);
    }
}