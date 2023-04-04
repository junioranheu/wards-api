namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData.Commands
{
    public interface IDeletarFeriadoDataCommand
    {
        Task ExecuteAsync(int feriadoId);
    }
}