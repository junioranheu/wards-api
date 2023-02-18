using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public interface ICriarRefreshTokenUsecase
    {
        Task<bool> Criar(RefreshToken input);
    }
}