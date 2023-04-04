using Wards.Application.UsesCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UsesCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UsesCases.FeriadosEstados.Shared.Output
{
    public sealed class FeriadoEstadoOutput
    {
        public int FeriadoEstadoId { get; set; }

        public int EstadoId { get; set; }
        public EstadoOutput? Estados { get; init; }

        public int FeriadoId { get; set; }
        public FeriadoOutput? Feriados { get; init; }
    }
}