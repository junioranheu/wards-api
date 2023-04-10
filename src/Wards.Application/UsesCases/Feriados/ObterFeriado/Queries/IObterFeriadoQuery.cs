using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.ObterFeriado.Queries
{
    public interface IObterFeriadoQuery
    {
        Task<Feriado?> Execute(int id);
    }
}
