namespace Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole.Commands
{
    public interface ICriarUsuarioRoleCommand
    {
        Task Execute(int[] rolesId, int usuarioId);
    }
}