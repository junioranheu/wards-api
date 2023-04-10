using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.ListarFeriado.Queries
{
    public interface IListarFeriadoQuery
    {
        Task<IEnumerable<Feriado>> Execute();
    }
}