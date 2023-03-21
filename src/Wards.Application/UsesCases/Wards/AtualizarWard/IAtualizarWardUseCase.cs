using Wards.Application.UsesCases.Wards.Shared.Input;

namespace Wards.Application.UsesCases.Wards.AtualizarWard
{
    public interface IAtualizarWardUseCase
    {
        Task<int> Execute(WardInput input);
    }
}