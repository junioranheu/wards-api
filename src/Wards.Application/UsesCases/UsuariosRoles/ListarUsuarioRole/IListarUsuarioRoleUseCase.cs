using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public interface IListarUsuarioRoleUseCase
    {
        Task<IEnumerable<UsuarioRole>> Execute(string email);
    }
}