using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public interface ICriarRefreshTokenUseCase
    {
        Task<bool> Criar(RefreshToken input);
    }
}