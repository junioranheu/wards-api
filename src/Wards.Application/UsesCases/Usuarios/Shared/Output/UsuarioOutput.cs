using System.Text.Json.Serialization;
using Wards.Domain.Entities;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.Shared.Output
{
    public sealed class UsuarioOutput
    {
        public int UsuarioId { get; set; }

        public string? NomeCompleto { get; set; } = string.Empty;

        public string? NomeUsuarioSistema { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Chamado { get; set; }

        [JsonIgnore]
        public string? HistPerfisAtivos { get; set; }

        public bool IsAtivo { get; set; } = true;

        public bool IsLatest { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();

        [JsonIgnore]
        public IEnumerable<UsuarioRole>? UsuarioRoles { get; init; }

        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;

        public int[]? UsuariosRolesId { get; set; } = null;
    }
}
