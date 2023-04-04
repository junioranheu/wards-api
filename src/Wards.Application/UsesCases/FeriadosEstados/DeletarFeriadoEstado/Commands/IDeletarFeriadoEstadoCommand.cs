namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado.Commands
{
    public interface IDeletarFeriadoEstadoCommand
    {
        Task ExecuteAsync(int feriadoId);
    }
}