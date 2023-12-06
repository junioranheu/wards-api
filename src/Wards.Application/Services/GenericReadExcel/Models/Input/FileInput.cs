using Microsoft.AspNetCore.Http;

namespace Wards.Application.Services.GenericReadExcel.Models.Input
{
    public sealed class FileInput
    {
        public required IFormFile FormFile { get; set; }
    }
}