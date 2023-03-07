using Microsoft.AspNetCore.Http;

namespace Wards.Application.Services.Import.CSV.Shared
{
    public sealed class ImportInput
    {
        public IFormFile? FormFile { get; set; }

        public string? Descricao { get; set; }
    }
}
