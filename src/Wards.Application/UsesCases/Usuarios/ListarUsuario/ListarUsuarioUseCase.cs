using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public sealed class ListarUsuarioUseCase : IListarUsuarioUseCase
    {
        public readonly IListarUsuarioQuery _listarQuery;

        public ListarUsuarioUseCase(IListarUsuarioQuery listarQuery)
        {
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<Usuario>> ExecuteAsync()
        {
            return await _listarQuery.ExecuteAsync();
        }
    }
}
