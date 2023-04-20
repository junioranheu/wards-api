namespace Wards.Application.UseCases.UsuariosRoles.CriarUsuarioRole.Commands
{
    public interface ICriarUsuarioRoleCommand
    {
        Task Execute(int[] rolesId, int usuarioId);
    }
}