using Microsoft.EntityFrameworkCore;

namespace Wards.Infrastructure.Data
{
    public class WardsContext : DbContext
    {
        public WardsContext(DbContextOptions<WardsContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
