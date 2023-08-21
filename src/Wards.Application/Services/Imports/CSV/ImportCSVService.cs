using Microsoft.AspNetCore.Http;

namespace Wards.Application.Services.Imports.CSV
{
    public sealed class ImportCSVService : IImportCSVService
    {
        public async Task ImportarCSV(IFormFile formFile, int usuarioId)
        {
            var a = 1;
        }
    }
}