using Wards.Application.UseCases.NewsLettersCadastros.Shared.Input;

namespace Wards.Application.UseCases.NewsLettersCadastros.CriarNewsLetterCadastro
{
    public interface ICriarNewsLetterCadastroUseCase
    {
        Task<int> Execute(NewsLetterCadastroInput input);
    }
}