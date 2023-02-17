using Dapper;
using System.Data;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public sealed class ObterUsuarioQuery : IObterUsuarioQuery
    {
        private readonly IDbConnection _dbConnection;

        public ObterUsuarioQuery(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Usuario> ExecuteAsync(int id)
        {
            string sql = "";

            return await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
        }   
    }
}
