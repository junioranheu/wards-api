using System.Text.Json.Serialization;

namespace Wards.Application.UseCases.Tokens.Shared.Input
{
    public sealed class RefreshTokenInput
    {
        public string? RefToken { get; set; } = string.Empty;

        [JsonIgnore]
        public int UsuarioId { get; set; }
    }
}