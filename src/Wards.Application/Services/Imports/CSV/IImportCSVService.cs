using Microsoft.AspNetCore.Http;

namespace Wards.Application.Services.Imports.CSV
{
    public interface IImportCSVService
    {
        Task ImportarCSV(IFormFile formFile, int usuarioId);
    }
}