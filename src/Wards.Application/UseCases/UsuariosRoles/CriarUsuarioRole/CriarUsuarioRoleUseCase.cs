using Wards.Application.UseCases.UsuariosRoles.CriarUsuarioRole.Commands;

namespace Wards.Application.UseCases.UsuariosRoles.CriarUsuarioRole
{
    public sealed class CriarUsuarioRoleUseCase : ICriarUsuarioRoleUseCase
    {
        private readonly ICriarUsuarioRoleCommand _criarUsuarioRoleCommand;

        public CriarUsuarioRoleUseCase(ICriarUsuarioRoleCommand criarUsuarioRoleCommand)
        {
            _criarUsuarioRoleCommand = criarUsuarioRoleCommand;
        }

        public async Task Execute(int[] rolesId, int usuarioId)
        {
            await _criarUsuarioRoleCommand.Execute(rolesId, usuarioId);
        }
    }
}
