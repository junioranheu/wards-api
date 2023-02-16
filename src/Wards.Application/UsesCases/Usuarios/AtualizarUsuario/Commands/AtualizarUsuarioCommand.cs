using Dapper;
using Wards.Application.UsesCases.Usuarios.Shared.Models;
using Wards.Infrastructure.Factory;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario.Commands
{
    public sealed class AtualizarUsuarioCommand : IAtualizarUsuarioCommand
    {
        private readonly IConnectionFactory _connectionFactory;

        public AtualizarUsuarioCommand(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> ExecuteAsync(UsuarioDTO dto)
        {
            string sql = "";

            return await _connectionFactory.CreateDbConnection().ExecuteAsync(sql, dto);
        }
    }
}
