namespace Wards.Application.UseCases.Logs.ExportarLog
{
    public interface IExportarLogUseCase
    {
        Task<byte[]?> ExecuteAsync(int usuarioId);
    }
}