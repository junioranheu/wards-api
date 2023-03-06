using Microsoft.EntityFrameworkCore;
using Wards.Infrastructure.Data;

namespace Wards.UnitTests
{
    public class Configuration
    {
        private readonly WardsContext _context;

        public Configuration()
        {
            var options = new DbContextOptionsBuilder<WardsContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            _context = new WardsContext(options);
        }

        public WardsContext CreateTestContext()
        {
            return _context;
        }
    }
}