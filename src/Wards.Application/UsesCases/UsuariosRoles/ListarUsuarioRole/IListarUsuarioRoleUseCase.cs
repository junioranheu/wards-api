using Wards.Application.UsesCases.UsuariosRoles.Shared.Output;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public interface IListarUsuarioRoleUseCase
    {
        Task<IEnumerable<UsuarioRoleOutput>?> Execute(string email);
    }
}