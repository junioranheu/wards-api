using System.ComponentModel.DataAnnotations;

namespace Wards.Domain.Entities
{
    public sealed class FeriadoData
    {
        [Key]
        public int FeriadoDataId { get; set; }

        public DateTime Data { get; set; }

        public int FeriadoId { get; set; }
        public Feriado? Feriados { get; init; }
    }
}