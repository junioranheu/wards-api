using Dapper;
using System.Data;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries
{
    public sealed class ObterUsuarioRoleQuery : IObterUsuarioRoleQuery
    {
        private readonly IDbConnection _dbConnection;

        public ObterUsuarioRoleQuery(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<UsuarioRole>> Obter(int id)
        {
            string sql = "";

            return await _dbConnection.QueryAsync<UsuarioRole>(sql);
        }

        public async Task<IEnumerable<UsuarioRole>> ObterByEmail(string email)
        {
            string sql = $@"SELECT UR.UsuarioRoleId, UR.UsuarioId, UR.RoleId
                            FROM UsuariosRoles UR
                            INNER JOIN Usuarios U ON U.UsuarioId = UR.UsuarioId
                            WHERE U.Email = '{email}';";

            return await _dbConnection.QueryAsync<UsuarioRole>(sql);
        }
    }
}
