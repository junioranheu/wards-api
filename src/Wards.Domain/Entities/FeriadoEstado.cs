using System.ComponentModel.DataAnnotations;

namespace Wards.Domain.Entities
{
    public sealed class FeriadoEstado
    {
        [Key]
        public int FeriadoEstadoId { get; set; }

        public int EstadoId { get; set; }
        public Estado? Estados { get; init; }

        public int FeriadoId { get; set; }
        public Feriado? Feriados { get; init; }
    }
}