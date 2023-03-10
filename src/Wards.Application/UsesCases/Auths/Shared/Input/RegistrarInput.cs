using System.Text.Json.Serialization;

namespace Wards.Application.UsesCases.Auths.Shared.Input
{
    public sealed class RegistrarInput
    {
        public string? NomeCompleto { get; set; } = string.Empty;

        public string? NomeUsuarioSistema { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Senha { get; set; } = string.Empty;

        public string? Chamado { get; set; } = string.Empty;

        [JsonIgnore]
        public string? HistPerfisAtivos { get; set; } = string.Empty;

        // Extra;
        public int[]? UsuariosRolesId { get; set; } = null;
    }
}