namespace Wards.Application.UseCases.Usuarios.Shared.Input
{
    public sealed class CriarRefreshTokenUsuarioInput
    {
        public string? Token { get; set; } = string.Empty;

        public string? RefreshToken { get; set; } = string.Empty;
    }
}