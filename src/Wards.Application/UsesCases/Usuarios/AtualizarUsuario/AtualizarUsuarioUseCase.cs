using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario
{
    public sealed class AtualizarUsuarioUseCase : IAtualizarUsuarioUseCase
    {
        public readonly IAtualizarUsuarioCommand _atualizarUsuarioCommand;

        public AtualizarUsuarioUseCase(IAtualizarUsuarioCommand atualizarUsuarioCommand)
        {
            _atualizarUsuarioCommand = atualizarUsuarioCommand;
        }

        public async Task<int> ExecuteAsync(UsuarioDTO dto)
        {
            return await _atualizarUsuarioCommand.ExecuteAsync(dto);
        }
    }
}
