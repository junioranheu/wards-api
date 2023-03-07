using Microsoft.AspNetCore.Http;
using System.Data;

namespace Wards.Application.Services.Import.CSV.Importar
{
    public interface IImportService
    {
        Task<(DataTable?, bool)> InserirCsv(string nomeDaTabela, object objectType, IFormFile formFile, int justificativaId, bool isVerificarData, List<string>? nomesEquipamentos = null);
    }
}