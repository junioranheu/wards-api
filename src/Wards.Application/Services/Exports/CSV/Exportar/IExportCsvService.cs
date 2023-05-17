namespace Wards.Application.Services.Exports.CSV.Exportar
{
    public interface IExportCsvService
    {
        byte[]? ConverterDadosParaCSVEmBytes<T>(List<T>? lista, string[,] colunas);
    }
}