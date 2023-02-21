using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public interface IListarUsuarioRoleUseCase
    {
        Task<IEnumerable<UsuarioRole>> ListarByEmail(string email);
        Task<IEnumerable<UsuarioRole>?> ListarUsuarioRolesByEmailComCache(dynamic context);
    }
}