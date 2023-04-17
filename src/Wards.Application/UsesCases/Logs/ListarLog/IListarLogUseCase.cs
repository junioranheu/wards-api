using Wards.Application.UsesCases.Logs.Shared.Output;
using Wards.Application.UsesCases.Shared.Models;

namespace Wards.Application.UsesCases.Logs.ListarLog
{
    public interface IListarLogUseCase
    {
        Task<IEnumerable<LogOutput>?> Execute(PaginacaoInput input);
    }
}