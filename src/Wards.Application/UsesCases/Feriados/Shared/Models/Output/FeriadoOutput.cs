using Wards.Application.UsesCases.Shared.Models;
using Wards.Domain.Entities;
using Wards.Domain.Enums;

namespace Wards.Application.UsesCases.Feriados.Shared.Models.Output
{
    public sealed class FeriadoOutput : ApiResponse
    {
        public int FeriadoId { get; set; }

        public TipoFeriadoEnum? Tipo { get; set; }

        public string? Nome { get; set; }

        public DateTime Data { get; set; }

        public bool IsMovel { get; set; }

        public bool Status { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime DataAtualizacao { get; set; }

        public IEnumerable<FeriadoData>? FeriadosDatas { get; init; }

        public IEnumerable<FeriadoEstado>? FeriadosEstados { get; init; }

        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; init; }

        public int? UsuarioIdMod { get; set; }
        public Usuario? UsuariosMods { get; init; }
    }
}