using System.ComponentModel;

namespace Wards.Domain.Enums
{
    public enum UsuarioRoleEnum
    {
        [Description("Administrador")]
        Administrador = 1,

        [Description("Usuário comum")]
        Comum = 2,

        [Description("Suporte")]
        Suporte = 3
    }
}