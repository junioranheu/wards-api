using Wards.Application.UsesCases.Usuarios.Shared.Input;

namespace Wards.Application.UsesCases.Auths.RefreshToken
{
    public interface IRefreshTokenUseCase
    {
        Task<(UsuarioInput?, string)> RefreshToken(string token, string refreshToken, string email);
    }
}