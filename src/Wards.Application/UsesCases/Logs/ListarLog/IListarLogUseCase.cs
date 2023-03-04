using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.ListarLog
{
    public interface IListarLogUseCase
    {
        Task<IEnumerable<Log>?> Execute();
    }
}