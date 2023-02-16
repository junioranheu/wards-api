using Wards.Application.UsesCases.Usuarios.AtualizarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario
{
    public sealed class AtualizarUsuarioUseCase : IAtualizarUsuarioUseCase
    {
        public readonly IAtualizarUsuarioCommand _atualizarCommand;

        public AtualizarUsuarioUseCase(IAtualizarUsuarioCommand atualizarCommand)
        {
            _atualizarCommand = atualizarCommand;
        }

        public async Task<int> ExecuteAsync(UsuarioDTO dto)
        {
            return await _atualizarCommand.ExecuteAsync(dto);
        }
    }
}
