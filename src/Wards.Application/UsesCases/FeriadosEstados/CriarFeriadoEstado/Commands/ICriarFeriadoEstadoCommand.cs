namespace Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado.Commands
{
    public interface ICriarFeriadoEstadoCommand
    {
        Task Execute(int[] estadoId, int feriadoId);
    }
}