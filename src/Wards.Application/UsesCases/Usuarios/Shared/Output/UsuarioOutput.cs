using Wards.Application.UsesCases.UsuariosRoles.Shared.Output;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.Shared.Output
{
    public sealed class UsuarioOutput
    {
        public int UsuarioId { get; set; }

        public string? NomeCompleto { get; set; } = string.Empty;

        public string? NomeUsuarioSistema { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Chamado { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();

        public IEnumerable<UsuarioRoleOutput>? UsuarioRoles { get; init; }

        // Extra;
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}