using Wards.Domain.Enums;

namespace Wards.Application.Services.Exports.XLSX.Exportar
{
    public interface IExportXLSXService
    {
        byte[]? ConverterDadosParaXLSXEmBytes<T>(List<T>? lista, string[,] colunas, string nomeSheet, bool isDataFormatoExport, string aplicarEstiloNasCelulas, TipoExportEnum? tipoExport = null);
    }
}