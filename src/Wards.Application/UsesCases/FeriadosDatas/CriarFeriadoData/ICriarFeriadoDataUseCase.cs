namespace Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData
{
    public interface ICriarFeriadoDataUseCase
    {
        Task ExecuteAsync(string[] data, int feriadoId);
    }
}