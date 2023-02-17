using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public interface IObterUsuarioRoleUseCase
    {
        Task<IEnumerable<UsuarioRole>> ObterById(int id);
        Task<IEnumerable<UsuarioRole>> ObterByEmail(string email);
        Task<IEnumerable<UsuarioRole>> ObterCacheObterUsuarioRolesByEmail(string email);
    }
}