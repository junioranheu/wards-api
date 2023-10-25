using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data.SqlClient;
using System.Diagnostics;

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
            string nomeConnectionString = _configuration["SystemSettings:NomeConnectionString"] ?? string.Empty;

            if (Debugger.IsAttached)
            {
                string? connectionString_secrets = _configuration[nomeConnectionString] ?? string.Empty; // secrets.json;
                return connectionString_secrets;
            }

            string connectionString = _configuration.GetConnectionString(nomeConnectionString) ?? string.Empty;

            return connectionString;
        }

        public SqlConnection ObterSqlServerConnection()
        {
            return new SqlConnection(ObterStringConnection());
        }

        public MySqlConnection ObterMySqlConnection()
        {
            return new MySqlConnection(ObterStringConnection());
        }
    }
}