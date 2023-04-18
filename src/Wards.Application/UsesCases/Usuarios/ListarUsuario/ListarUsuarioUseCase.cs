using AutoMapper;
using Wards.Application.UsesCases.Shared.Models;
using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public sealed class ListarUsuarioUseCase : IListarUsuarioUseCase
    {
        private readonly IMapper _map;
        private readonly IListarUsuarioQuery _listarQuery;

        public ListarUsuarioUseCase(IMapper map, IListarUsuarioQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<UsuarioOutput>?> Execute(PaginacaoInput input)
        {
            return _map.Map<IEnumerable<UsuarioOutput>>(await _listarQuery.Execute(input));
        }
    }
}