using Wards.Application.UsesCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UseCases.Feriados.ObterFeriado
{
    public interface IObterFeriadoUseCase
    {
        Task<FeriadoOutput> ExecuteAsync(int id);
    }
}