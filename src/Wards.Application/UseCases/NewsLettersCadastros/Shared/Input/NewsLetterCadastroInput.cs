namespace Wards.Application.UseCases.NewsLettersCadastros.Shared.Input
{
    public sealed class NewsLetterCadastroInput
    {
        public string Email { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; }
    }
}