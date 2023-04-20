using Wards.Application.UseCases.Shared.Models;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Logs.ListarLog.Queries
{
    public interface IListarLogQuery
    {
        Task<IEnumerable<Log>> Execute(PaginacaoInput input);
    }
}