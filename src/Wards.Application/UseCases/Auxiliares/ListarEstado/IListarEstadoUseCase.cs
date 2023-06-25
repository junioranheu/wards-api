using Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.Auxiliares.ListarEstado
{
    public interface IListarEstadoUseCase
    {
        Task<IEnumerable<EstadoOutput>> Execute(PaginacaoInput input);
    }
}