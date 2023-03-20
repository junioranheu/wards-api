using Wards.Application.UsesCases.Wards.Shared.Output;

namespace Wards.Application.UsesCases.Wards.ObterWard
{
    public interface IObterWardUseCase
    {
        Task<WardOutput?> Execute(int id);
    }
}