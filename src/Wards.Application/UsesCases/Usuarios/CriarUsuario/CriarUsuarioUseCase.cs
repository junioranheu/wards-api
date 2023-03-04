using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public sealed class CriarUsuarioUseCase : ICriarUsuarioUseCase
    {
        private readonly ICriarUsuarioCommand _criarCommand;

        public CriarUsuarioUseCase(ICriarUsuarioCommand criarCommand)
        {
            _criarCommand = criarCommand;
        }

        public async Task<int> Criar(Usuario input)
        {
            return await _criarCommand.Criar(input);
        }
    }
}
