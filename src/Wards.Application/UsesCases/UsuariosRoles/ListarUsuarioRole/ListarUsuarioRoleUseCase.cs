using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public sealed class ListarUsuarioRoleUseCase : IListarUsuarioRoleUseCase
    {
        private readonly IListarUsuarioRoleQuery _obterQuery;

        public ListarUsuarioRoleUseCase(IListarUsuarioRoleQuery obterQuery)
        {
            _obterQuery = obterQuery;
        }

        public async Task<IEnumerable<UsuarioRole>> Execute(string email)
        {
            return await _obterQuery.Execute(email);
        }
    }
}