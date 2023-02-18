using Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands;
using Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public sealed class CriarRefreshTokenUsecase : ICriarRefreshTokenUsecase
    {
        public readonly CriarRefreshTokenCommand _criarCommand;
        public readonly DeletarRefreshTokenCommand _deletarCommand;

        public CriarRefreshTokenUsecase(CriarRefreshTokenCommand criarCommand, DeletarRefreshTokenCommand deletarCommand)
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
