namespace Wards.Application.UseCases.Auths.Shared.Output
{
    public sealed class CriarRefreshTokenUsuarioOutput
    {
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}