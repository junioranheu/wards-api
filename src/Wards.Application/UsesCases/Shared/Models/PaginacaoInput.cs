namespace Wards.Application.UsesCases.Shared.Models
{
    public sealed class PaginacaoInput
    {
        const int primeiraPagina = 0;
        const int quantidadeRegistros = 10;

        public int Pagina { get; set; } = primeiraPagina;

        public int Limit { get; set; } = quantidadeRegistros;

        public bool IsSelectAll { get; set; } = false;
    }
}