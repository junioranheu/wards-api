using Newtonsoft.Json;

namespace Wards.Application.UsesCases.Usuarios.Shared.Input
{
    public sealed class UsuarioInput
    {
        [JsonIgnore]
        public int? UsuarioId { get; set; } = 0;

        public string? NomeCompleto { get; set; } = string.Empty;

        public string? NomeUsuarioSistema { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Senha { get; set; } = string.Empty;

        public string? Chamado { get; set; } = string.Empty;

        [JsonIgnore]
        public string? HistPerfisAtivos { get; set; } = string.Empty;

        // Extra;
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;

        public int[]? UsuariosRolesId { get; set; } = null;
    }
}