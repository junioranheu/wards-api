using Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands;
using Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public sealed class CriarRefreshTokenUseCase : ICriarRefreshTokenUseCase
    {
        private readonly ICriarRefreshTokenCommand _criarCommand;
        private readonly IDeletarRefreshTokenCommand _deletarCommand;

        public CriarRefreshTokenUseCase(ICriarRefreshTokenCommand criarCommand, IDeletarRefreshTokenCommand deletarCommand)
        {
            _criarCommand = criarCommand;
            _deletarCommand = deletarCommand;
        }

        public async Task<bool> Execute(RefreshToken input)
        {
            await _deletarCommand.Execute(input);
            return await _criarCommand.Execute(input);
        }
    }
}
