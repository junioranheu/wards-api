namespace Wards.Application.UsesCases.Auths.RefreshToken.Models
{
    public sealed class RefreshTokenOutput
    {
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}
