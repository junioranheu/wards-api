using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.ListarLog.Queries
{
    public interface IListarLogQuery
    {
        Task<IEnumerable<Log>> Listar();
    }
}