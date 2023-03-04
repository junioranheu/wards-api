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

        public async Task<IEnumerable<Usuario>?> Listar()
        {
            string sql = $@"SELECT * FROM Usuarios WHERE IsAtivo = 1;";
            var usuarios = await _dbConnection.QueryAsync<Usuario>(sql);

            return usuarios;
        }
    }
}
