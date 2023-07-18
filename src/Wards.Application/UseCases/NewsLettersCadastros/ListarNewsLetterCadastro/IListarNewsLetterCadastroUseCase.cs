using Wards.Application.UseCases.NewsLettersCadastros.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro
{
    public interface IListarNewsLetterCadastroUseCase
    {
        Task<IEnumerable<NewsLetterCadastroOutput>> Execute(PaginacaoInput input);
    }
}