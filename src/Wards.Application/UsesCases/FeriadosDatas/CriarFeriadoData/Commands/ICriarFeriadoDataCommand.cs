namespace Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands
{
    public interface ICriarFeriadoDataCommand
    {
        Task ExecuteAsync(string[] data, int feriadoId);
    }
}