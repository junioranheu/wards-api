using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands
{
    public interface ICriarRefreshTokenCommand
    {
        Task Execute(RefreshToken input);
    }
}