using Wards.Application.UsesCases.Auths.RefreshToken.Models;

namespace Wards.Application.UsesCases.Auths.RefreshToken
{
    public interface IRefreshTokenUseCase
    {
        Task<(RefreshTokenOutput?, string)> Execute(string token, string refreshToken, string email);
    }
}