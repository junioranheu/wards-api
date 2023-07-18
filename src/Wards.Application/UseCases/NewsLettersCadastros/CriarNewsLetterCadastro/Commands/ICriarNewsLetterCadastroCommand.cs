using Wards.Domain.Entities;

namespace Wards.Application.UseCases.NewsLettersCadastros.CriarNewsLetterCadastro.Commands
{
    public interface ICriarNewsLetterCadastroCommand
    {
        Task<int> Execute(NewsLetterCadastro input);
    }
}