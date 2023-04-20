using AutoMapper;
using Wards.Application.UseCases.UsuariosRoles.ObterUsuarioRole.Queries;
using Wards.Application.UseCases.UsuariosRoles.Shared.Output;

namespace Wards.Application.UseCases.UsuariosRoles.ObterUsuarioRole
{
    public sealed class ListarUsuarioRoleUseCase : IListarUsuarioRoleUseCase
    {
        private readonly IMapper _map;
        private readonly IListarUsuarioRoleQuery _listarQuery;

        public ListarUsuarioRoleUseCase(IMapper map, IListarUsuarioRoleQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<UsuarioRoleOutput>?> Execute(string email)
        {
            return _map.Map<IEnumerable<UsuarioRoleOutput>>(await _listarQuery.Execute(email));
        }
    }
}