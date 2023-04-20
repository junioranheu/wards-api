using Wards.Application.UseCases.Tokens.Shared.Input;

namespace Wards.Application.UseCases.Tokens.CriarRefreshToken
{
    public interface ICriarRefreshTokenUseCase
    {
        Task Execute(RefreshTokenInput input);
    }
}