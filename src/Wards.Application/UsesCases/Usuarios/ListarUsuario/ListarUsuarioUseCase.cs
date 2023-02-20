using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Domain.DTOs;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public sealed class ListarUsuarioUseCase : IListarUsuarioUseCase
    {
        public readonly IListarUsuarioQuery _listarQuery;

        public ListarUsuarioUseCase(IListarUsuarioQuery listarQuery)
        {
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<UsuarioDTO>> Listar()
        {
            return await _listarQuery.Listar();
        }
    }
}
