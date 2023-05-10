using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.Shared.Models;

namespace Wards.Application.UseCases.Logs.ListarLog
{
    public interface IListarLogUseCase
    {
        Task<IEnumerable<LogOutput>> Execute(PaginacaoInput input);
    }
}