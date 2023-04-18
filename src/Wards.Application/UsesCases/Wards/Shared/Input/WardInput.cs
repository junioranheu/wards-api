using System.Text.Json.Serialization;

namespace Wards.Application.UsesCases.Wards.Shared.Input
{
    public sealed class WardInput
    {
        public int WardId { get; set; }

        public string? Titulo { get; set; }

        public string? Conteudo { get; set; }

        [JsonIgnore]
        public int? UsuarioId { get; set; }

        [JsonIgnore]
        public int? UsuarioModId { get; set; }
    }
}