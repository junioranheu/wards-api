namespace Wards.Application.UseCases.UsuariosRoles.CriarUsuarioRole
{
    public interface ICriarUsuarioRoleUseCase
    {
        Task Execute(int[] rolesId, int usuarioId);
    }
}