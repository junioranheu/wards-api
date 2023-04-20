using Wards.Application.UseCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UseCases.Feriados.ObterFeriado
{
    public interface IObterFeriadoUseCase
    {
        Task<FeriadoOutput> Execute(int id);
    }
}