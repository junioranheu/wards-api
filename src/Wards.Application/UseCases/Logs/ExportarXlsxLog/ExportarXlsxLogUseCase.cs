using Wards.Application.UseCases.Logs.ExportarXlsxLog.Commands;

namespace Wards.Application.UseCases.Logs.ExportarXlsxLog
{
    public sealed class ExportarXlsxLogUseCase : IExportarXlsxLogUseCase
    {
        private readonly IExportarXlsxLogCommand _exportarCommand;

        public ExportarXlsxLogUseCase(IExportarXlsxLogCommand exportarCommand)
        {
            _exportarCommand = exportarCommand;
        }

        public async Task<byte[]?> ExecuteAsync(int usuarioId, bool isTodos)
        {
            return await _exportarCommand.Execute(usuarioId, isTodos);
        }
    }
}