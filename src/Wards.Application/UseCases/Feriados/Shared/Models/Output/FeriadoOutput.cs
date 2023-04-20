using Wards.Application.UseCases.FeriadosDatas.Shared.Output;
using Wards.Application.UseCases.FeriadosEstados.Shared.Output;
using Wards.Application.UseCases.Shared.Models;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Domain.Enums;

namespace Wards.Application.UseCases.Feriados.Shared.Models.Output
{
    public sealed class FeriadoOutput : ApiOutput
    {
        public int FeriadoId { get; set; }

        public TipoFeriadoEnum? Tipo { get; set; }

        public string? Nome { get; set; }

        public bool IsMovel { get; set; }

        public bool IsAtivo { get; set; }

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