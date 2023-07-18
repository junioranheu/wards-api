using Wards.Application.UseCases.NewslettersCadastros.Shared.Input;

namespace Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro
{
    public interface ICriarNewsletterCadastroUseCase
    {
        Task<int> Execute(NewsletterCadastroInput input);
    }
}