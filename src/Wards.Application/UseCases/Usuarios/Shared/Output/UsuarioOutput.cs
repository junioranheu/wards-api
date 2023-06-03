using Wards.Application.UseCases.UsuariosRoles.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.Shared.Output
{
    public sealed class UsuarioOutput
    {
        public int UsuarioId { get; set; }

        public string? NomeCompleto { get; set; } = string.Empty;

        public string? NomeUsuarioSistema { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Foto { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; }

        public IEnumerable<UsuarioRoleOutput>? UsuarioRoles { get; init; }
    }
}