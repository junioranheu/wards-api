using System.Text.Json.Serialization;

namespace Wards.Application.UsesCases.Tokens.Shared.Input
{
    public sealed class RefreshTokenInput
    {
        public string? RefToken { get; set; } = null;

        [JsonIgnore]
        public int UsuarioId { get; set; }
    }
}
