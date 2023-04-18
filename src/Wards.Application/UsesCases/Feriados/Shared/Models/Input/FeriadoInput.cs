using System.Text.Json.Serialization;
using Wards.Domain.Enums;

namespace Wards.Application.UsesCases.Feriados.Shared.Models.Input
{
    public class FeriadoInput
    {
        public int FeriadoId { get; set; }

        public TipoFeriadoEnum? Tipo { get; set; }

        public string? Nome { get; set; }

        public bool IsMovel { get; set; }

        public bool IsAtivo { get; set; }

        [JsonIgnore]
        public int? UsuarioId { get; set; }

        [JsonIgnore]
        public int? UsuarioIdMod { get; set; }

        // Extras;
        public string[]? Data { get; set; }

        public int[]? DistribuidoraId { get; set; }

        public int[]? EstadoId { get; set; }
    }
}