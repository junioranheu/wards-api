using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Token.CriarRefreshToken
{
    public interface ICriarRefreshTokenUsecase
    {
        Task<bool> Criar(RefreshToken input);
    }
}