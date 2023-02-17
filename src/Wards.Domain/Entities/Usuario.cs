using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        public string? NomeCompleto { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Senha { get; set; } = string.Empty;

        public int UsuarioRoleId { get; set; }
        public UsuarioRole? UsuarioRoles { get; set; }

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();
    }
}
