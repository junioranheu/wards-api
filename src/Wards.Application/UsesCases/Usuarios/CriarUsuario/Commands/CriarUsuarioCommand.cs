using Dapper;
using System.Data;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public sealed class CriarUsuarioCommand : ICriarUsuarioCommand
    {
        private readonly IDbConnection _dbConnection;

        public CriarUsuarioCommand(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> Criar(Usuario dto)
        {
            string sql = "";

            return await _dbConnection.ExecuteAsync(sql, dto);
        }
    }
}
