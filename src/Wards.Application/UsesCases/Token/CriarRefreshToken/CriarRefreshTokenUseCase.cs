using Wards.Application.UsesCases.Token.CriarRefreshToken.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Token.CriarRefreshToken
{
    public sealed class CriarRefreshTokenUsecase : ICriarRefreshTokenUsecase
    {
        public readonly CriarRefreshTokenCommand _criarCommand;

        public CriarRefreshTokenUsecase(CriarRefreshTokenCommand criarCommand)
        {
            _criarCommand = criarCommand;
        }

        public async Task<bool> Criar(RefreshToken input)
        {
            return await _criarCommand.Criar(input);
        }
    }
}
