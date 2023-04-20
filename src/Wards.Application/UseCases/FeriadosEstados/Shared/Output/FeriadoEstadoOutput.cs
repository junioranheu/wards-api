using Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UseCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UseCases.FeriadosEstados.Shared.Output
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