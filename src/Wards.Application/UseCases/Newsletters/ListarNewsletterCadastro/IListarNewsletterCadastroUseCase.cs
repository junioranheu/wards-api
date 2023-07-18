using Wards.Application.UseCases.NewslettersCadastros.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro
{
    public interface IListarNewsletterCadastroUseCase
    {
        Task<IEnumerable<NewsletterCadastroOutput>> Execute(PaginacaoInput input);
    }
}