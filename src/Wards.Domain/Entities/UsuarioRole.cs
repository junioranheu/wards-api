using System.ComponentModel.DataAnnotations;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class UsuarioRole
    {
        [Key]
        public int UsuarioRoleId { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; set; }

        public UsuarioRoleEnum RoleId { get; set; }
        public Role? Roles { get; set; }

        public DateTime Data { get; set; } = HorarioBrasilia();
    }
}
