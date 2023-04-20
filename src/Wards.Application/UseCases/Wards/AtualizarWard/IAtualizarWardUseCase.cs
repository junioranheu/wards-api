using Wards.Application.UseCases.Wards.Shared.Input;

namespace Wards.Application.UseCases.Wards.AtualizarWard
{
    public interface IAtualizarWardUseCase
    {
        Task<int> Execute(WardInput input);
    }
}