using Dapper;
using System.Data;
using Wards.Domain.DTOs;
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

        public async Task Criar(Usuario input)
        {
            string sql = "";
            await _dbConnection.ExecuteAsync(sql, input);
        }
    }
}
