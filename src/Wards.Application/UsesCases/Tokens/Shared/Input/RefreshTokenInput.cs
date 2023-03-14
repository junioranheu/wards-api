namespace Wards.Application.UsesCases.Tokens.Shared.Input
{
    public sealed class RefreshTokenInput
    {
        public string? RefToken { get; set; } = null;

        public int UsuarioId { get; set; }
    }
}
