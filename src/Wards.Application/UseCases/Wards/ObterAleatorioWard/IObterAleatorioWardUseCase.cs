using Wards.Application.UseCases.Wards.Shared.Output;

namespace Wards.Application.UseCases.Wards.ObterAleatorioWard
{
    public interface IObterAleatorioWardUseCase
    {
        Task<WardOutput?> Execute();
    }
}