using Microsoft.AspNetCore.Http;

namespace Wards.Application.Services.Imports.Shared.Models.Input
{
    public sealed class ImportCSVInput
    {
        public IFormFile? FormFile { get; set; }

        public object? ClasseAlvo { get; set; }

        public int? UsuarioId { get; set; }
    }
}