using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.Logs.ListarLog
{
    public interface IListarLogUseCase
    {
        Task<IEnumerable<LogOutput>> Execute(PaginacaoInput input);
    }
}