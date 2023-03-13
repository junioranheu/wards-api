using Wards.Application.UsesCases.Logs.Shared.Output;

namespace Wards.Application.UsesCases.Logs.ListarLog
{
    public interface IListarLogUseCase
    {
        Task<IEnumerable<LogOutput>?> Execute();
    }
}