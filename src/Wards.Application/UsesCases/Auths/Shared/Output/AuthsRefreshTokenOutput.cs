namespace Wards.Application.UsesCases.Auths.Shared.Output
{
    public sealed class AuthsRefreshTokenOutput
    {
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}
