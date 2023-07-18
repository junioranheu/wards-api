using System.Text.Json.Serialization;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.UseCases.NewslettersCadastros.Shared.Input
{
    public sealed class NewsletterCadastroInput
    {
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsAtivo { get; set; } = true;

        [JsonIgnore]
        public DateTime Data { get; set; } = GerarHorarioBrasilia();
    }
}