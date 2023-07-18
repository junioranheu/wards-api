namespace Wards.Application.UseCases.NewslettersCadastros.Shared.Output
{
    public sealed class NewsletterCadastroOutput
    {
        public string Email { get; set; } = string.Empty;

        public DateTime Data { get; set; }
    }
}