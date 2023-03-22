using Wards.Application.UsesCases.Shared.Models;

namespace Wards.Application.UsesCases.Auths.Shared.Output
{
    public sealed class AuthsRefreshTokenOutput : ApiResponse
    {
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}