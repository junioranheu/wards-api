using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.ObterFeriado.Queries
{
    public interface IObterFeriadoQuery
    {
        Task<Feriado?> ExecuteAsync(int id);
    }
}
