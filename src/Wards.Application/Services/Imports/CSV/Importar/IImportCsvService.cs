using Microsoft.AspNetCore.Http;
using System.Data;

namespace Wards.Application.Services.Imports.CSV.Importar
{
    public interface IImportCsvService
    {
        Task<(DataTable? tabelaErros, bool isErroBanco)> InserirCsv(string nomeDaTabela, object objectType, IFormFile formFile, int justificativaId, bool isVerificarData, List<string>? nomesEquipamentos = null);
    }
}