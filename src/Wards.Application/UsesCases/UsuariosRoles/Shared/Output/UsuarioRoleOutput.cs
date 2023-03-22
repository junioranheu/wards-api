using Wards.Application.UsesCases.Shared.Models;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Entities;
using Wards.Domain.Enums;

namespace Wards.Application.UsesCases.UsuariosRoles.Shared.Output
{
    public sealed class UsuarioRoleOutput : ApiResponse
    {
        public int UsuarioId { get; set; }
        public UsuarioOutput? Usuarios { get; set; }

        public UsuarioRoleEnum RoleId { get; set; }
        public Role? Roles { get; set; }
    }
}