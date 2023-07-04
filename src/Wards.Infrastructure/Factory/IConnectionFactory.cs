using MySqlConnector;
using System.Data.SqlClient;

namespace Wards.Infrastructure.Factory
{
    public interface IConnectionFactory
    {
        MySqlConnection ObterMySqlConnection();
        SqlConnection ObterSqlServerConnection();
        string ObterStringConnection();
    }
}