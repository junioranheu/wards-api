using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public sealed class ObterUsuarioQuery : IObterUsuarioQuery
    {
        private readonly IConnectionFactory _connectionFactory;

        public ObterUsuarioQuery(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Usuario> ExecuteAsync(int id)
        {
            string sql = "";

            return await _connectionFactory.CreateDbConnection().QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
        }
    }
}
