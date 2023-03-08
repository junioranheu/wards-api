using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public sealed class ListarUsuarioUseCase : IListarUsuarioUseCase
    {
        private readonly IListarUsuarioQuery _listarQuery;

        public ListarUsuarioUseCase(IListarUsuarioQuery listarQuery)
        {
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<Usuario>?> Execute()
        {
            return await _listarQuery.Execute();
        }
    }
}