using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Tokens.DeletarRefreshToken.Commands
{
    public interface IDeletarRefreshTokenCommand
    {
        Task Execute(RefreshToken input);
    }
}