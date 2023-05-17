namespace Wards.Application.Services.Exports.XLSX.Exportar
{
    public interface IExportXlsxService
    {
        byte[]? ConverterDadosParaXLSXEmBytes<T>(List<T>? lista, string[,] colunas, string nomeSheet, bool isDataFormatoExport, string aplicarEstiloNasCelulas, int tipoRowInicial = 0);
    }
}