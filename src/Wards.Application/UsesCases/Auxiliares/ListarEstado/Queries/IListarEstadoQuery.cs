using Wards.Application.UsesCases.Shared.Models;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries
{
    public interface IListarEstadoQuery
    {
        Task<IEnumerable<Estado>> Execute(PaginacaoInput input);
    }
}