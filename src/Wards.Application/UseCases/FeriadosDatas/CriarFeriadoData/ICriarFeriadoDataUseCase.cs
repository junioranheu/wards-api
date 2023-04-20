namespace Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData
{
    public interface ICriarFeriadoDataUseCase
    {
        Task Execute(string[] data, int feriadoId);
    }
}