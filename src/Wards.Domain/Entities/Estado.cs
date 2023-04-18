using System.ComponentModel.DataAnnotations;

namespace Wards.Domain.Entities
{
    public sealed class Estado
    {
        [Key]
        public int EstadoId { get; set; }

        public string? Nome { get; set; } = string.Empty;

        public string? Sigla { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;
    }
}