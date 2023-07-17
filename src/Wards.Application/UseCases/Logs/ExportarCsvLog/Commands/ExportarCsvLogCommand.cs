using Microsoft.EntityFrameworkCore;
using Wards.Application.Services.Exports.CSV.Exportar;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Logs.ExportarCsvLog.Commands
{
    public sealed class ExportarCsvLogCommand : IExportarCsvLogCommand
    {
        private readonly WardsContext _context;
        private readonly IExportCsvService _exportCsvService;

        public ExportarCsvLogCommand(WardsContext context, IExportCsvService exportCsvService)
        {
            _context = context;
            _exportCsvService = exportCsvService;
        }

        public async Task<byte[]?> Execute(int usuarioId, bool isTodos)
        {
            List<Log>? listaLogs = await ListarLogs(usuarioId, isTodos);

            string[,] colunas = {
                { "ID do log", nameof(Log.LogId) }, { "Tipo da requisição", nameof(Log.TipoRequisicao) },
                { "End-point", nameof(Log.Endpoint) }, { "Parâmetros", nameof(Log.Parametros) },
                { "Descrição", nameof(Log.Descricao) }, { "Status da resposta", nameof(Log.StatusResposta) },
                { "ID do usuário", nameof(Log.UsuarioId) }, { "Data", nameof(Log.Data) }
            };

            byte[]? csv = _exportCsvService.ConverterDadosParaCSVEmBytes(lista: listaLogs, colunas: colunas);

            return csv;
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