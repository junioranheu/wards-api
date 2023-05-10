namespace Wards.Application.UseCases.Logs.ExportarLog.Commands
{
    public interface IExportarLogCommand
    {
        Task<byte[]?> Execute(int usuarioId);
    }
}