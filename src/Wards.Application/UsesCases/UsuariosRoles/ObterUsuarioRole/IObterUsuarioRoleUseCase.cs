using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public interface IObterUsuarioRoleUseCase
    {
        Task<IEnumerable<UsuarioRole>> Obter(int id);
        Task<IEnumerable<UsuarioRole>> ObterByEmail(string email);
        Task<IEnumerable<UsuarioRole>?> ObterUsuarioRolesByEmailComCache(dynamic context);
    }
}