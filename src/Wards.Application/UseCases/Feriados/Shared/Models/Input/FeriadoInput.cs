using System.Text.Json.Serialization;
using Wards.Domain.Enums;

namespace Wards.Application.UseCases.Feriados.Shared.Models.Input
{
    public class FeriadoInput
    {
        public int FeriadoId { get; set; }

        public TipoFeriadoEnum? Tipo { get; set; }

        public string Nome { get; set; } = string.Empty;

        public bool IsMovel { get; set; }

        public bool IsAtivo { get; set; }

        [JsonIgnore]
        public int? UsuarioId { get; set; }

        [JsonIgnore]
        public int? UsuarioIdMod { get; set; }

        // Propriedades extras;
        public string[]? Data { get; set; }

        public int[]? DistribuidoraId { get; set; }

        public int[]? EstadoId { get; set; }
    }
}