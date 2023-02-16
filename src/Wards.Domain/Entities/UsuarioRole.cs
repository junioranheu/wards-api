using System.ComponentModel.DataAnnotations;

namespace Wards.Domain.Entities
{
    public class UsuarioRole
    {
        [Key]
        public int UsuarioRoleId { get; set; }

        public string? Tipo { get; set; } = null;

        public string? Descricao { get; set; } = null;

        public bool IsAtivo { get; set; } = true;

        public DateTime DataRegistro { get; set; } = HorarioBrasilia();
    }
}
