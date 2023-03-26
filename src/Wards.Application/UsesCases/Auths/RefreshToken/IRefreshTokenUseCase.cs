using Wards.Application.UsesCases.Auths.Shared.Output;

namespace Wards.Application.UsesCases.Auths.RefreshToken
{
    public interface IRefreshTokenUseCase
    {
        Task<AuthsRefreshTokenOutput?> Execute(string token, string refreshToken, string email);
    }
}