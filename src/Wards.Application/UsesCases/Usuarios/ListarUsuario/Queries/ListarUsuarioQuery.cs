using Dapper;
using System.Data;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries
{
    public sealed class ListarUsuarioQuery : IListarUsuarioQuery
    {
        private readonly IDbConnection _dbConnection;

        public ListarUsuarioQuery(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Usuario>> ExecuteAsync()
        {
            string sql = $@"SELECT * FROM Usuarios WHERE IsAtivo = 1;";

            return await _dbConnection.QueryAsync<Usuario>(sql);
        }
    }
}
