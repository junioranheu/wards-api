namespace Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries
{
    public sealed class ListarUsuarioQuery : IListarUsuarioQuery
    {
        private readonly IConnectionFactory _connectionFactory;

        public ListarUsuarioQuery(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<IEnumerable<Usuario>> ExecuteAsync()
        {
            string sql = "";

            return await _connectionFactory.CreateDbConnection().QueryAsync<Usuario>(sql);
        }
    }
}
