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
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<NewsletterCadastro> NewslettersCadastros { get; set; }

        // Feriados;
        public DbSet<Feriado> Feriados { get; set; }
        public DbSet<FeriadoData> FeriadosDatas { get; set; }
        public DbSet<FeriadoEstado> FeriadosEstados { get; set; }

        // Wards;
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<WardHashtag> WardsHashtags { get; set; }

        // Usuários;
        public DbSet<Role> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioRole> UsuariosRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}