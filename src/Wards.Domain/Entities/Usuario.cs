using Wards.Domain.Enums;

namespace Wards.Domain.Entities
{
    public sealed class Usuario
    {
        public int UsuarioId { get; set; }

        public string? Nome { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public UsuarioRoleEnum Role { get; set; } = UsuarioRoleEnum.Comum;

        public StatusAtivoInativoEnum IsAtivo { get; set; } = StatusAtivoInativoEnum.Ativo;
    }
}
