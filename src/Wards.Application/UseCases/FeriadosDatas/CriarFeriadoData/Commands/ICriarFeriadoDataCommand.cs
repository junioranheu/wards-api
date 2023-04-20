namespace Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands
{
    public interface ICriarFeriadoDataCommand
    {
        Task Execute(string[] data, int feriadoId);
    }
}