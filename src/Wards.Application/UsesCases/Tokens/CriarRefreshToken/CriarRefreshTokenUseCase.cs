using Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public sealed class CriarRefreshTokenUseCase : ICriarRefreshTokenUseCase
    {
        public readonly ICriarRefreshTokenUseCase _criarCommand;
        public readonly IDeletarRefreshTokenCommand _deletarCommand;

        public CriarRefreshTokenUseCase(ICriarRefreshTokenUseCase criarCommand, IDeletarRefreshTokenCommand deletarCommand)
        {
            _criarCommand = criarCommand;
            _deletarCommand = deletarCommand;
        }

        public async Task<bool> Criar(RefreshToken input)
        {
            await _deletarCommand.Deletar(input);
            return await _criarCommand.Criar(input);
        }
    }
}
