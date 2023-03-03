namespace Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole.Commands
{
    public interface ICriarUsuarioRoleCommand
    {
        Task Criar(int[] rolesId, int usuarioId);
    }
}