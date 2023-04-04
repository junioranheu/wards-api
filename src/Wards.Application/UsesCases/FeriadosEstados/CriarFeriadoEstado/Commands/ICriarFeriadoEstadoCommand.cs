namespace Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado.Commands
{
    public interface ICriarFeriadoEstadoCommand
    {
        Task ExecuteAsync(int[] estadoId, int feriadoId);
    }
}