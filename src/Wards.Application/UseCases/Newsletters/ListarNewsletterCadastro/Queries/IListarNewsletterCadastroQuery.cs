using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro.Queries
{
    public interface IListarNewsletterCadastroQuery
    {
        Task<IEnumerable<NewsletterCadastro>> Execute(PaginacaoInput input);
    }
}