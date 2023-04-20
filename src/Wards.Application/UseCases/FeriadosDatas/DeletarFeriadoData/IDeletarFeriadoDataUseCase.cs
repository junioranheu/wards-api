namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData
{
    public interface IDeletarFeriadoDataUseCase
    {
        Task Execute(int feriadoId);
    }
}