using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Ward
    {
        [Key]
        public int WardId { get; set; }

        public string? Conteudo { get; set; }

        public int? UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuarios { get; init; }

        public DateTime Data { get; set; } = HorarioBrasilia();

        public bool IsAtivo { get; set; } = true;
    }
}