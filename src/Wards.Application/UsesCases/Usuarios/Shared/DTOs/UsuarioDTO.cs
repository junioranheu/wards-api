using Wards.Domain.DTOs;
using Wards.Domain.Enums;

namespace Wards.Application.UsesCases.Usuarios.Shared.Models
{
    public sealed class UsuarioDTO : _RetornoApiDTO
    {
        public int UsuarioId { get; set; }

        public string? Nome { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public UsuarioRoleEnum Role { get; set; } = UsuarioRoleEnum.Comum;

        public bool IsAtivo { get; set; } = true;
    }
}
