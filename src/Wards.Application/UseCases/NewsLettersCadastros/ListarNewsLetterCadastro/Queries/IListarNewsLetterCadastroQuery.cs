using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro.Queries
{
    public interface IListarNewsLetterCadastroQuery
    {
        Task<IEnumerable<NewsLetterCadastro>> Execute(PaginacaoInput input);
    }
}