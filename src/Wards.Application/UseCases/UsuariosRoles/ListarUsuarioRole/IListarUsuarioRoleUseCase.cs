using Wards.Application.UseCases.UsuariosRoles.Shared.Output;

namespace Wards.Application.UseCases.UsuariosRoles.ObterUsuarioRole
{
    public interface IListarUsuarioRoleUseCase
    {
        Task<IEnumerable<UsuarioRoleOutput>?> Execute(string email);
    }
}