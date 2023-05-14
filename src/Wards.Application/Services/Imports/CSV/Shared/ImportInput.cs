using Microsoft.AspNetCore.Http;

namespace Wards.Application.Services.Imports.CSV.Shared
{
    public sealed class ImportInput
    {
        public IFormFile? FormFile { get; set; }

        public string? Descricao { get; set; }
    }
}
