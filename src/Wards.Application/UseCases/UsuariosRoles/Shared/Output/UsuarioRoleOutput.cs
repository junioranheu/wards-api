using Wards.Domain.Entities;
using Wards.Domain.Enums;

namespace Wards.Application.UseCases.UsuariosRoles.Shared.Output
{
    public sealed class UsuarioRoleOutput
    {
        public int UsuarioId { get; set; }

        public UsuarioRoleEnum RoleId { get; set; }
        public Role? Roles { get; set; }
    }
}