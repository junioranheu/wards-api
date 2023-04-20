namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData.Commands
{
    public interface IDeletarFeriadoDataCommand
    {
        Task Execute(int feriadoId);
    }
}