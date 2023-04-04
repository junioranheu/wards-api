namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData
{
    public interface IDeletarFeriadoDataUseCase
    {
        Task ExecuteAsync(int feriadoId);
    }
}