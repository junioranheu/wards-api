namespace Wards.Application.UsesCases.Auths.Shared.Input
{
    public sealed class AuthsRefreshTokenInput
    {
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}