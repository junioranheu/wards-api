using Microsoft.AspNetCore.Http;

namespace Wards.Application.UsesCases.Shared.Models
{
    public class ApiResponse
    {
        public string[]? Messages { get; set; } = Array.Empty<string>();
    }
}