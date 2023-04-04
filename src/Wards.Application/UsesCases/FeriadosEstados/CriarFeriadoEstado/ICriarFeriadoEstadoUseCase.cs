namespace Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado
{
    public interface ICriarFeriadoEstadoUseCase
    {
        Task ExecuteAsync(int[] estadoId, int feriadoId);
    }
}