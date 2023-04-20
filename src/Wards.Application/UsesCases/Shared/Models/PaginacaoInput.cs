namespace Wards.Application.UsesCases.Shared.Models
{
    public class PaginacaoInput
    {
        const int primeiraPagina = 0;
        const int quantidadeRegistros = 10;

        public int Index { get; set; } = primeiraPagina;

        public int Limit { get; set; } = quantidadeRegistros;

        public bool IsSelectAll { get; set; } = false;
    }
}