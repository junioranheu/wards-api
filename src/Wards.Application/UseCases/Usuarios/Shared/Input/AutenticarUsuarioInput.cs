namespace Wards.Application.UseCases.Usuarios.Shared.Input
{
    public sealed class AutenticarUsuarioInput
    {
        public string? Login { get; set; } = string.Empty;

        public string? Senha { get; set; } = string.Empty;
    }
}
