using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public interface ICriarRefreshTokenUseCase
    {
        Task Execute(RefreshToken input);
    }
}