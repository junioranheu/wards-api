using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public sealed class CriarUsuarioUseCase : ICriarUsuarioUseCase
    {
        public readonly ICriarUsuarioCommand _criarCommand;

        public CriarUsuarioUseCase(ICriarUsuarioCommand criarCommand)
        {
            _criarCommand = criarCommand;
        }

        public async Task<UsuarioDTO> Criar(Usuario input)
        {
            return await _criarCommand.Criar(input);
        }
    }
}
