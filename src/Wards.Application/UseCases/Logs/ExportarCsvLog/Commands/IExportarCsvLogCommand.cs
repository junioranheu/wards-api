namespace Wards.Application.UseCases.Logs.ExportarCsvLog.Commands
{
    public interface IExportarCsvLogCommand
    {
        Task<byte[]?> Execute(int usuarioId, bool isTodos);
    }
}