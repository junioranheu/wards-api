using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }

        public string? RefToken { get; set; } = string.Empty;

        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; set; }

        public DateTime DataRegistro { get; set; } = HorarioBrasilia();
    }
}
