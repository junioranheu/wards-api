namespace Wards.Application.UseCases.Logs.ExportarXlsxLog
{
    public interface IExportarXlsxLogUseCase
    {
        Task<byte[]?> ExecuteAsync(int usuarioId, bool isTodos);
    }
}