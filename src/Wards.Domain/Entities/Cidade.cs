using System.ComponentModel.DataAnnotations;

namespace Wards.Domain.Entities
{
    public sealed class Cidade
    {
        [Key]
        public int CidadeId { get; set; }

        public string? Nome { get; set; } = string.Empty;

        public int EstadoId { get; set; }
        public Estado? Estados { get; set; }

        public bool IsAtivo { get; set; } = true;
    }
}