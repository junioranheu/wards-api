using Wards.Application.UsesCases.Token.CriarRefreshToken.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Token.CriarRefreshToken
{
    public sealed class CriarRefreshTokenUsecase
    {
        public readonly CriarRefreshTokenCommand _criarCommand;

        public CriarRefreshTokenUsecase(CriarRefreshTokenCommand criarCommand)
        {
            _criarCommand = criarCommand;
        }

        public async Task Criar(RefreshToken input)
        {
            await _criarCommand.Criar(input);
        }
    }
}
