using Wards.Application.UsesCases.Shared.Models;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.ListarFeriado.Queries
{
    public interface IListarFeriadoQuery
    {
        Task<IEnumerable<Feriado>> Execute(PaginacaoInput input);
    }
}