using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public sealed class CriarUsuarioUseCase : ICriarUsuarioUseCase
    {
        public readonly ICriarUsuarioCommand _criarUsuarioCommand;

        public CriarUsuarioUseCase(ICriarUsuarioCommand criarUsuarioCommand)
        {
            _criarUsuarioCommand = criarUsuarioCommand;
        }

        public async Task<int> ExecuteAsync(UsuarioDTO dto)
        {
            return await _criarUsuarioCommand.ExecuteAsync(dto);
        }
    }
}
