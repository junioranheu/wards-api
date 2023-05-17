using Wards.Application.UseCases.Logs.ExportarCsvLog.Commands;

namespace Wards.Application.UseCases.Logs.ExportarCsvLog
{
    public sealed class ExportarCsvLogUseCase : IExportarCsvLogUseCase
    {
        private readonly IExportarCsvLogCommand _exportarCommand;

        public ExportarCsvLogUseCase(IExportarCsvLogCommand exportarCommand)
        {
            _exportarCommand = exportarCommand;
        }

        public async Task<byte[]?> ExecuteAsync(int usuarioId, bool isTodos)
        {
            return await _exportarCommand.Execute(usuarioId, isTodos);
        }
    }
}