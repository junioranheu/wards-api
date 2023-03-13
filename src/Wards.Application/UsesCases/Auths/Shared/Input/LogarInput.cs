namespace Wards.Application.UsesCases.Auths.Shared.Input
{
    public sealed class LogarInput
    {
        public string? NomeUsuarioSistema { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Senha { get; set; } = string.Empty;
    }
}
