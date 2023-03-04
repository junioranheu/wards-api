using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.DeletarRefreshToken
{
    public interface IDeletarRefreshTokenUseCase
    {
        Task Execute(RefreshToken input);
    }
}