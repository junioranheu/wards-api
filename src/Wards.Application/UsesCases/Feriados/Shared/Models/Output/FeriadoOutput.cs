using Wards.Application.UsesCases.FeriadosDatas.Shared.Output;
using Wards.Application.UsesCases.FeriadosEstados.Shared.Output;
using Wards.Application.UsesCases.Shared.Models;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;

namespace Wards.Application.UsesCases.Feriados.Shared.Models.Output
{
    public sealed class FeriadoOutput : ApiOutput
    {
        public int FeriadoId { get; set; }

        public TipoFeriadoEnum? Tipo { get; set; }

        public string? Nome { get; set; }

        public bool IsMovel { get; set; }

        public bool Status { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime DataAtualizacao { get; set; }

        public IEnumerable<FeriadoDataOutput>? FeriadosDatas { get; init; }

        public IEnumerable<FeriadoEstadoOutput>? FeriadosEstados { get; init; }

        public int UsuarioId { get; set; }
        public UsuarioOutput? Usuarios { get; init; }

        public int? UsuarioIdMod { get; set; }
        public UsuarioOutput? UsuariosMods { get; init; }
    }
}