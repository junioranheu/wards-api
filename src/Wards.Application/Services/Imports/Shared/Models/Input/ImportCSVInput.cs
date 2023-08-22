using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Wards.Application.Services.Imports.Shared.Models.Input
{
    public sealed class ImportCSVInput
    {
        public required IFormFile FormFile { get; set; }

        public bool IsPularCabecalho { get; set; } = true;

        public string? Separador { get; set; } = ";";

        [JsonIgnore]
        public object? ClasseAlvo { get; set; }

        [JsonIgnore]
        public int? UsuarioId { get; set; }
    }
}