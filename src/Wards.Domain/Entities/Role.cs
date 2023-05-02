using System.ComponentModel.DataAnnotations;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Role
    {
        [Key]
        public UsuarioRoleEnum RoleId { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();
    }
}