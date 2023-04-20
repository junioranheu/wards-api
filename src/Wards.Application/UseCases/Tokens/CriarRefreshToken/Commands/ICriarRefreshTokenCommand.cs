using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Tokens.CriarRefreshToken.Commands
{
    public interface ICriarRefreshTokenCommand
    {
        Task Execute(RefreshToken input);
    }
}