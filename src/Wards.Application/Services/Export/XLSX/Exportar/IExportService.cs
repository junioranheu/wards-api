namespace Wards.Application.Services.Export.XLSX.Exportar
{
    public interface IExportService
    {
        byte[]? ConverterDadosParaXLSXEmBytes<T>(List<T>? lista, string[,] colunas, string nomeSheet, bool isDataFormatoExport, string aplicarEstiloNasCelulas, int tipoRowInicial = 0);
    }
}