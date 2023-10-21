using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data.SqlClient;

namespace Wards.Infrastructure.Factory.ConnectionFactory
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public ConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ObterStringConnection()
        {
            string? secretSenhaBancoDados = _configuration["SecretSenhaBancoDados"]; // secrets.json;
            string? con = _configuration.GetConnectionString(_configuration["SystemSettings:NomeConnectionString"] ?? string.Empty);
            con = con?.Replace("[SecretSenhaBancoDados]", secretSenhaBancoDados);

            return con ?? string.Empty;
        }

        public SqlConnection ObterSqlServerConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("con"));
        }

        public MySqlConnection ObterMySqlConnection()
        {
            return new MySqlConnection(ObterStringConnection());
        }
    }
}