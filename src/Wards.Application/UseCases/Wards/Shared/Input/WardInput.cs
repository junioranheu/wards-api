using System.Text.Json.Serialization;
using Wards.Application.UseCases.Usuarios.Shared.Input;

namespace Wards.Application.UseCases.Wards.Shared.Input
{
    public sealed class WardInput
    {
        [JsonIgnore]
        public int WardId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        [JsonIgnore]
        public byte[]? ImagemPrincipalBlob { get; set; } = Array.Empty<byte>();

        public string Conteudo { get; set; } = string.Empty;

        [JsonIgnore]
        public int? UsuarioId { get; set; }

        [JsonIgnore]
        public CriarUsuarioInput? Usuarios { get; set; }

        [JsonIgnore]
        public int? UsuarioModId { get; set; }

        [JsonIgnore]
        public DateTime? DataMod { get; set; }

        [JsonIgnore]
        public bool? IsAtivo { get; set; } = true;
    }
}