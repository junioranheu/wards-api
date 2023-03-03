using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.Shared.Input
{
    public sealed class UsuarioInput
    {
        public Usuario? Usuarios { get; init; }

        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;

        public int[]? UsuariosRolesId { get; set; } = null;
    }
}
