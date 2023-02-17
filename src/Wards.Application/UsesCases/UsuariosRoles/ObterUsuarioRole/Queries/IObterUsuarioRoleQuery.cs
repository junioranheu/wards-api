using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries
{
    public interface IObterUsuarioRoleQuery
    {
        Task<IEnumerable<UsuarioRole>> ObterById(int id);
        Task<IEnumerable<UsuarioRole>> ObterByEmail(string email);
    }
}