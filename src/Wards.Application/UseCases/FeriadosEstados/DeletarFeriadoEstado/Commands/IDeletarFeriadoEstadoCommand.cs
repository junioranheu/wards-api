namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado.Commands
{
    public interface IDeletarFeriadoEstadoCommand
    {
        Task Execute(int feriadoId);
    }
}