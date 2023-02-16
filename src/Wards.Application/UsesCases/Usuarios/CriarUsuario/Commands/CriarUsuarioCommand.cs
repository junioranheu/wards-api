using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public sealed class CriarUsuarioCommand : ICriarUsuarioCommand
    {
        private readonly IConnectionFactory _connectionFactory;

        public CriarUsuarioCommand(IConnectionFactory connectionFactory)
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
