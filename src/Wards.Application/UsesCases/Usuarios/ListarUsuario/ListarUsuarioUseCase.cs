using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public sealed class ListarUsuarioUseCase : IListarUsuarioUseCase
    {
        public readonly IListarUsuarioQuery _listarUsuarioQuery;

        public ListarUsuarioUseCase(IListarUsuarioQuery listarUsuarioQuery)
        {
            _listarUsuarioQuery = listarUsuarioQuery;
        }

        public async Task<IEnumerable<Usuario>> ExecuteAsync()
        {
            return await _listarUsuarioQuery.ExecuteAsync();
        }
    }
}
