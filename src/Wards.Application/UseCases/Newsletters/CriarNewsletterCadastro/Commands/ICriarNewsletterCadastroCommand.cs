using Wards.Domain.Entities;

namespace Wards.Application.UseCases.NewslettersCadastros.CriarNewsletterCadastro.Commands
{
    public interface ICriarNewsletterCadastroCommand
    {
        Task<int> Execute(NewsletterCadastro input);
    }
}