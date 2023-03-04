namespace Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole
{
    public interface ICriarUsuarioRoleUseCase
    {
        Task Execute(int[] rolesId, int usuarioId);
    }
}