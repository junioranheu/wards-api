using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Auths.RefreshToken
{
    public interface IRefreshTokenUseCase
    {
        Task<(UsuarioOutput?, string)> Execute(string token, string refreshToken, string email);
    }
}