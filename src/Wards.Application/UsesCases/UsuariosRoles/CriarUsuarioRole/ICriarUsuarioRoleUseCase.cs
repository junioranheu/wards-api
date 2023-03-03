namespace Wards.Application.UsesCases.UsuariosRoles.CriarUsuarioRole
{
    public interface ICriarUsuarioRoleUseCase
    {
        Task Criar(int[] rolesId, int usuarioId);
    }
}