using Microsoft.AspNetCore.Http;

namespace Wards.Application.UsesCases.Shared.Models
{
    public class ApiResponse
    {
        public int Code { get; set; } = StatusCodes.Status200OK;

        public Guid Request_Id { get; set; } = Guid.NewGuid();

        public string[]? Messages { get; set; } = Array.Empty<string>();
    }
}