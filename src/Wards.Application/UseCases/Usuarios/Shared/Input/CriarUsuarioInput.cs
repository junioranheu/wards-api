using System.Text.Json.Serialization;

namespace Wards.Application.UseCases.Usuarios.Shared.Input
{
    public sealed class CriarUsuarioInput
    {
        public string NomeCompleto { get; set; } = string.Empty;

        public string NomeUsuarioSistema { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public string Chamado { get; set; } = string.Empty;

        [JsonIgnore]
        public string? Foto { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsVerificado { get; set; } = false;

        [JsonIgnore]
        public string? CodigoVerificacao { get; set; } = string.Empty;

        [JsonIgnore]
        public DateTime? ValidadeCodigoVerificacao { get; set; }

        [JsonIgnore]
        public string? HistPerfisAtivos { get; set; } = string.Empty;

        // Extra;
        public int[]? UsuariosRolesId { get; set; } = null;
    }
}