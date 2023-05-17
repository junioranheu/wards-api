using Microsoft.EntityFrameworkCore;
using Wards.Application.Services.Exports.XLSX.Exportar;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Logs.ExportarXlsxLog.Commands
{
    public sealed class ExportarXlsxLogCommand : IExportarXlsxLogCommand
    {
        private readonly WardsContext _context;
        private readonly IExportXlsxService _exportXlsxService;

        public ExportarXlsxLogCommand(WardsContext context, IExportXlsxService exportXlsxService)
        {
            _context = context;
            _exportXlsxService = exportXlsxService;
        }

        public async Task<byte[]?> Execute(int usuarioId, bool isTodos)
        {
            List<Log>? listaLogs = await ListarLogs(usuarioId, isTodos);

            string[,] colunas = {
                { "ID do log", nameof(Log.LogId), string.Empty }, { "Tipo da requisição", nameof(Log.TipoRequisicao), string.Empty },
                { "End-point", nameof(Log.Endpoint), string.Empty }, { "Parâmetros", nameof(Log.Parametros), string.Empty },
                { "Descrição", nameof(Log.Descricao), string.Empty }, { "Status da resposta", nameof(Log.StatusResposta), string.Empty },
                { "ID do usuário", nameof(Log.UsuarioId), string.Empty }, { "Usuário", nameof(Log.Usuarios), nameof(Log.Usuarios.NomeUsuarioSistema) },
                { "Data", nameof(Log.Data), string.Empty }
            };

            byte[]? xlsx = _exportXlsxService.ConverterDadosParaXLSXEmBytes(lista: listaLogs,
                colunas: colunas, nomeSheet: "Logs", isDataFormatoExport: false, aplicarEstiloNasCelulas: "A1:I1", tipoRowInicial: 0);

            return xlsx;
        }

        private async Task<List<Log>?> ListarLogs(int usuarioId, bool isTodos)
        {
            var listaLogs = await _context.Logs.
                                  Include(u => u.Usuarios).
                                  Where(l => (isTodos ? true : l.UsuarioId == usuarioId)).
                                  AsNoTracking().ToListAsync();

            return listaLogs;
        }
    }
}