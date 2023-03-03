using AutoMapper;
using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

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

        public async Task<IEnumerable<UsuarioDTO>> Listar()
        {
            IEnumerable<Usuario?> usuarios = await _listarQuery.Listar();
            return _map.Map<IEnumerable<UsuarioDTO>>(usuarios);
        }
    }
}
