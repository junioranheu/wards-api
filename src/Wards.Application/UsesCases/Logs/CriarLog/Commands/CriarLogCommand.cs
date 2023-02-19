using Dapper;
using System.Data;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.CriarLog.Commands
{
    public sealed class CriarLogCommand : ICriarLogCommand
    {
        private readonly IDbConnection _dbConnection;

        public CriarLogCommand(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> Criar(Log input)
        {
            string sql = $@"INSERT INTO Logs (TipoRequisicao, Endpoint, Parametros, StatusResposta, UsuarioRoleId, Data)
                            VALUES('{input.TipoRequisicao}', '{input.Endpoint}', '{input.Parametros}', 
                                   {input.StatusResposta}, {input.UsuarioRoleId}, NOW());";

            return await _dbConnection.ExecuteAsync(sql, input);
        }
    }
}
