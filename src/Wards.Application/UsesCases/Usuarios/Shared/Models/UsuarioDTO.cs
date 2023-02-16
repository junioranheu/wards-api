namespace Wards.Application.UsesCases.Usuarios.Shared.Models
{
    public sealed class UsuarioDTO
    {
        public string? Nome { get; set; }
        public string? Perfil { get; set; }
        public string? Matricula { get; set; }
        public StatusAtivoInativoEnum Status { get; set; }
    }
}
