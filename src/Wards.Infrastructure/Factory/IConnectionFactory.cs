using System.Data;

namespace Wards.Infrastructure.Factory
{
    public interface IConnectionFactory
    {
        IDbConnection CreateDbConnection();
    }
}