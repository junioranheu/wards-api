namespace Wards.Application.UseCases.NewsLettersCadastros.Shared.Output
{
    public sealed class NewsLetterCadastroOutput
    {
        public string Email { get; set; } = string.Empty;

        public DateTime Data { get; set; }
    }
}