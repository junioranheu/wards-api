namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado
{
    public interface IDeletarFeriadoEstadoUseCase
    {
        Task ExecuteAsync(int feriadoId);
    }
}