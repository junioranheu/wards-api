using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public sealed class CriarUsuarioUseCase : ICriarUsuarioUseCase
    {
        public readonly ICriarUsuarioCommand _criarCommand;

        public CriarUsuarioUseCase(ICriarUsuarioCommand criarCommand)
        {
            _criarCommand = criarCommand;
        }

        public async Task<int> ExecuteAsync(UsuarioDTO dto)
        {
            return await _criarCommand.ExecuteAsync(dto);
        }
    }
}
