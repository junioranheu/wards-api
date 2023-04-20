namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado
{
    public interface IDeletarFeriadoEstadoUseCase
    {
        Task Execute(int feriadoId);
    }
}