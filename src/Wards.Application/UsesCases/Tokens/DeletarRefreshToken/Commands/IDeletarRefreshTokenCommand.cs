using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands
{
    public interface IDeletarRefreshTokenCommand
    {
        Task<bool> Deletar(RefreshToken input);
    }
}