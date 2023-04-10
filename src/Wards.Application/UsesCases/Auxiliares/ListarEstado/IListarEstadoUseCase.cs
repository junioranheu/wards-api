using Wards.Application.UsesCases.Auxiliares.ListarEstado.Shared.Output;

namespace Wards.Application.UsesCases.Auxiliares.ListarEstado
{
    public interface IListarEstadoUseCase
    {
        Task<IEnumerable<EstadoOutput>> Execute();
    }
}