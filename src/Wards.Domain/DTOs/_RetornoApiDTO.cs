namespace Wards.Domain.DTOs
{
    public class _RetornoApiDTO
    {
        public bool Erro { get; set; } = false;
        public int CodigoErro { get; set; } = 0;
        public string? MensagemErro { get; set; } = string.Empty;
    }
}
