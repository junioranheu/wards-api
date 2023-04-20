using Wards.Application.UseCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Tokens.DeletarRefreshToken
{
    public sealed class DeletarRefreshTokenUseCase : IDeletarRefreshTokenUseCase
    {
        private readonly IDeletarRefreshTokenCommand _deletarCommand;

        public DeletarRefreshTokenUseCase(IDeletarRefreshTokenCommand deletarCommand)
        {
            _deletarCommand = deletarCommand;
        }

        public async Task Execute(RefreshToken input)
        {
            await _deletarCommand.Execute(input);
        }
    }
}
