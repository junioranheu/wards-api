using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands
{
    public interface ICriarRefreshTokenCommand
    {
        Task<bool> Execute(RefreshToken input);
    }
}