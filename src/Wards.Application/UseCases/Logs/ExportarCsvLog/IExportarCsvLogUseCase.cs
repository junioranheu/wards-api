namespace Wards.Application.UseCases.Logs.ExportarCsvLog
{
    public interface IExportarCsvLogUseCase
    {
        Task<byte[]?> ExecuteAsync(int usuarioId, bool isTodos);
    }
}