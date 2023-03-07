using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace Wards.Infrastructure.Factory
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public ConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateDbStringConnection()
        {
            var secretSenhaBancoDados = _configuration["SecretSenhaBancoDados"]; // secrets.json;
            string con = _configuration.GetConnectionString(_configuration["SystemSettings:NomeConnectionString"]!)!;
            con = con.Replace("[SecretSenhaBancoDados]", secretSenhaBancoDados);

            return con;
        }

        public IDbConnection CreateDbConnection()
        {
            // return new SqlConnection(_configuration.GetConnectionString("CPFL"));
            return new MySqlConnection(CreateDbStringConnection());
        }

        public MySqlConnection CreateDbSqlConnection()
        {
            // return new SqlConnection(_configuration.GetConnectionString("CPFL"));
            return new MySqlConnection(CreateDbStringConnection());
        }
    }
}