using Wards.Application.UseCases.Wards.Shared.Input;

namespace Wards.Application.UseCases.Wards.CriarWard
{
    public interface ICriarWardUseCase
    {
        Task<int> Execute(WardInput input);
    }
}