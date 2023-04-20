using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Tokens.DeletarRefreshToken
{
    public interface IDeletarRefreshTokenUseCase
    {
        Task Execute(RefreshToken input);
    }
}