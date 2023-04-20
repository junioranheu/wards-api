using Wards.Application.UseCases.Wards.Shared.Output;

namespace Wards.Application.UseCases.Wards.ObterWard
{
    public interface IObterWardUseCase
    {
        Task<WardOutput?> Execute(int id);
    }
}