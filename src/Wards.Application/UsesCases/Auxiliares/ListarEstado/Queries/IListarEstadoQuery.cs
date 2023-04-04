using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries
{
    public interface IListarEstadoQuery
    {
        Task<IEnumerable<Estado>> ExecuteAsync();
    }
}