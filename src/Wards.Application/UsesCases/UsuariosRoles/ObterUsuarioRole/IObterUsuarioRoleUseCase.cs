using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public interface IObterUsuarioRoleUseCase
    {
        Task<IEnumerable<UsuarioRole>> ObterByUsuarioEmail(string email);
        Task<IEnumerable<UsuarioRole>?> ObterUsuarioRolesByEmailComCache(dynamic context);
    }
}