using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data.SqlClient;

namespace Wards.Infrastructure.Factory
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
            var secretSenhaBancoDados = _configuration["SecretSenhaBancoDados"]; // secrets.json;
            string con = _configuration.GetConnectionString(_configuration["SystemSettings:NomeConnectionString"]!)!;
            con = con.Replace("[SecretSenhaBancoDados]", secretSenhaBancoDados);

            return con;
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