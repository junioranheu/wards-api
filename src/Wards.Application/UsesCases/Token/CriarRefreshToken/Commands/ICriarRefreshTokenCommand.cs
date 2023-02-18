using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Token.CriarRefreshToken.Commands
{
    public interface ICriarRefreshTokenCommand
    {
        Task<bool> Criar(RefreshToken input);
    }
}