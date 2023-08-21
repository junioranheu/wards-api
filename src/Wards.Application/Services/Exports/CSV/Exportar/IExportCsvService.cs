namespace Wards.Application.Services.Exports.CSV.Exportar
{
    public interface IExportCSVService
    {
        byte[]? ConverterDadosParaCSVEmBytes<T>(List<T>? lista, string[,] colunas);
    }
}