using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Ward
    {
        [Key]
        public int WardId { get; set; }

        public string? Titulo { get; set; } = string.Empty;

        public string? Conteudo { get; set; } = string.Empty;

        public int? UsuarioId { get; set; }
        public Usuario? Usuarios { get; init; }

        public DateTime Data { get; set; } = HorarioBrasilia();

        public int? UsuarioModId { get; set; }
        [ForeignKey(nameof(UsuarioModId))]
        public Usuario? UsuariosMods { get; init; }

        public DateTime? DataMod { get; set; } = null;

        public bool IsAtivo { get; set; } = true;
    }
}