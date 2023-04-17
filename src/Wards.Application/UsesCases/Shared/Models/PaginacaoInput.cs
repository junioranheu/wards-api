namespace Wards.Application.UsesCases.Shared.Models
{
    public sealed class PaginacaoInput
    {
        public int Pagina { get; set; } = 0;

        public int QtdItens { get; set; } = 10;

        public bool IsGetAll { get; set; } = false;
    }
}