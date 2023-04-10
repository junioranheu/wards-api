namespace Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado
{
    public interface ICriarFeriadoEstadoUseCase
    {
        Task Execute(int[] estadoId, int feriadoId);
    }
}