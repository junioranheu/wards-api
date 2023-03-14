using Wards.Application.UsesCases.Tokens.Shared.Input;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public interface ICriarRefreshTokenUseCase
    {
        Task Execute(RefreshTokenInput input);
    }
}