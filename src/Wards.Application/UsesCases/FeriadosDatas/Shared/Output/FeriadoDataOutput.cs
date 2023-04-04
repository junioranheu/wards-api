using Wards.Application.UsesCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UsesCases.FeriadosDatas.Shared.Output
{
    public sealed class FeriadoDataOutput
    {
        public int FeriadoDataId { get; set; }

        public DateTime Data { get; set; }

        public int FeriadoId { get; set; }
        public FeriadoOutput? Feriados { get; init; }
    }
}