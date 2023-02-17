using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace Wards.Infrastructure.Factory
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly WebApplicationBuilder _builder;

        public ConnectionFactory(IConfiguration configuration, WebApplicationBuilder builder)
        {
            _configuration = configuration;
            _builder = builder;
        }

        public IDbConnection CreateDbConnection()
        {
            var secretSenhaBancoDados = _builder.Configuration["SecretSenhaBancoDados"]; // secrets.json;
            string con = _builder.Configuration.GetConnectionString(_builder.Configuration["SystemSettings:NomeConnectionString"] ?? string.Empty) ?? string.Empty;
            con = con.Replace("[SecretSenhaBancoDados]", secretSenhaBancoDados);

            // return new SqlConnection(_configuration.GetConnectionString(con)); // SQL Server;
            return new MySqlConnection(_configuration.GetConnectionString(con)); // MySQL;
        }
    }
}
