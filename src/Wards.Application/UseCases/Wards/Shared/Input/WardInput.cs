using System.Text.Json.Serialization;

namespace Wards.Application.UseCases.Wards.Shared.Input
{
    public sealed class WardInput
    {
        public int WardId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Conteudo { get; set; } = string.Empty;

        [JsonIgnore]
        public int? UsuarioId { get; set; }

        [JsonIgnore]
        public int? UsuarioModId { get; set; }
    }
}