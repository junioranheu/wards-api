namespace Wards.Application.UseCases.Logs.ExportarXlsxLog.Commands
{
    public interface IExportarXlsxLogCommand
    {
        Task<byte[]?> Execute(int usuarioId, bool isTodos);
    }
}