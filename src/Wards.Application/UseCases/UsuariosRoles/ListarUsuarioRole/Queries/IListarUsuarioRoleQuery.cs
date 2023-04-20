using Wards.Domain.Entities;

namespace Wards.Application.UseCases.UsuariosRoles.ObterUsuarioRole.Queries
{
    public interface IListarUsuarioRoleQuery
    {
        Task<IEnumerable<UsuarioRole>> Execute(string email);
    }
}