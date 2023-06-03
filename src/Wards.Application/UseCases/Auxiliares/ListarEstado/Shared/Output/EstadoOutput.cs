namespace Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output
{
    public sealed class EstadoOutput
    {
        public int EstadoId { get; set; }

        public string? Nome { get; set; }

        public string? Sigla { get; set; }

        public bool? IsAtivo { get; set; }
    }
}