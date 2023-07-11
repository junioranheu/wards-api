using MySqlConnector;
using System.Data.SqlClient;

namespace Wards.Infrastructure.Factory.ConnectionFactory
{
    public interface IConnectionFactory
    {
        MySqlConnection ObterMySqlConnection();
        SqlConnection ObterSqlServerConnection();
        string ObterStringConnection();
    }
}