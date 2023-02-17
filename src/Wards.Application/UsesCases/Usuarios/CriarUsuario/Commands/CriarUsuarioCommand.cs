using Dapper;
using System.Data;
using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public sealed class CriarUsuarioCommand : ICriarUsuarioCommand
    {
        private readonly IDbConnection _dbConnection;

        public CriarUsuarioCommand(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> ExecuteAsync(UsuarioDTO dto)
        {
            string sql = "";

            return await _dbConnection.ExecuteAsync(sql, dto);
        }
    }
}
