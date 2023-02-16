using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;

namespace Wards.Infrastructure.Data
{
    public class WardsContext : DbContext
    {
        public WardsContext(DbContextOptions<WardsContext> options) : base(options)
        {

        }

        // Outros;
        public DbSet<Log> Logs { get; set; }

        // Usuários;
        public DbSet<UsuarioRole> UsuariosRoles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
