using Wards.Application.UseCases.Logs.ExportarLog.Commands;

namespace Wards.Application.UseCases.Logs.ExportarLog
{
    public sealed class ExportarLogUseCase : IExportarLogUseCase
    {
        private readonly IExportarLogCommand _exportarCommand;

        public ExportarLogUseCase(IExportarLogCommand exportarCommand)
        {
            _exportarCommand = exportarCommand;
        }

        public async Task<byte[]?> ExecuteAsync(int usuarioId)
        {
            return await _exportarCommand.Execute(usuarioId);
        }
    }
}