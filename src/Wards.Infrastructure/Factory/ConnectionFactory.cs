using Microsoft.Extensions.Configuration;
using System.Data;
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

        public IDbConnection CreateDbConnection()
        {
            string con = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SystemSettings")["NomeConnectionString"] ?? string.Empty;
            return new SqlConnection(_configuration.GetConnectionString(con));
        }
    }
}
