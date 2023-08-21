namespace Wards.Application.Services.Exports.CSV
{
    public interface IExportCSVService
    {
        byte[]? ConverterDadosParaCSVEmBytes<T>(List<T>? lista, string[,] colunas);
    }
}