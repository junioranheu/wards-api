using System.ComponentModel;

namespace Wards.Domain.Enums
{
    public enum UsuarioRoleEnum
    {
        [Description("Administrador")]
        Adm = 1,

        [Description("Usuário comum")]
        Comum = 2
    }
}
