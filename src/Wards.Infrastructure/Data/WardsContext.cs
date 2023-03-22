using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;

namespace Wards.Infrastructure.Data
{
    public class WardsContext : DbContext
    {
        public WardsContext(DbContextOptions<WardsContext> options) : base(options)
        {

        }

        // Exemplos;
        public DbSet<CsvImportExemploUsuario> CsvImportExemploUsuarios { get; set; }

        // Outros;
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Ward> Wards { get; set; }

        // Usuários;
        public DbSet<Role> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioRole> UsuariosRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}