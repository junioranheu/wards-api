using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Feriado
    {
        [Key]
        public int FeriadoId { get; set; }

        public TipoFeriadoEnum? Tipo { get; set; }

        public string Nome { get; set; } = string.Empty;

        public bool IsMovel { get; set; } = false;

        public bool IsAtivo { get; set; } = true;

        public DateTime DataCriacao { get; set; } = HorarioBrasilia();

        public DateTime? DataAtualizacao { get; set; } = null;

        public IEnumerable<FeriadoData>? FeriadosDatas { get; init; }

        public IEnumerable<FeriadoEstado>? FeriadosEstados { get; init; }

        [ForeignKey(nameof(Usuarios))]
        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; init; }

        [ForeignKey(nameof(UsuariosMods))]
        public int? UsuarioIdMod { get; set; }
        [ForeignKey(nameof(UsuarioIdMod))]
        public Usuario? UsuariosMods { get; init; }
    }
}