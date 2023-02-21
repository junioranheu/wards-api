using Wards.Domain.DTOs;

namespace Wards.Application.UsesCases.Auths.RefreshToken
{
    public interface IRefreshTokenUseCase
    {
        Task<UsuarioDTO> RefreshToken(string token, string refreshToken);
    }
}