using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Role
    {
        [Key]
        public int RoleId { get; set; }

        public string? Tipo { get; set; } = null;

        public string? Descricao { get; set; } = null;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();
    }
}
