using Wards.Application.UseCases.Shared.Models;

namespace Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output
{
    public sealed class EstadoOutput : ApiOutput
    {
        public int EstadoId { get; set; }

        public string? Nome { get; set; }

        public string? Sigla { get; set; }

        public bool? IsAtivo { get; set; }
    }
}