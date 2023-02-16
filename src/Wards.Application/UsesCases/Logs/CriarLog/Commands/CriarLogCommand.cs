using Dapper;
using Wards.Domain.Entities;
using Wards.Infrastructure.Factory;

namespace Wards.Application.UsesCases.Logs.CriarLog.Commands
{
    public sealed class CriarLogCommand : ICriarLogCommand
    {
        private readonly IConnectionFactory _connectionFactory;

        public CriarLogCommand(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> ExecuteAsync(Log dto)
        {
            string sql = "";

            return await _connectionFactory.CreateDbConnection().ExecuteAsync(sql, dto);
        }
    }
}
