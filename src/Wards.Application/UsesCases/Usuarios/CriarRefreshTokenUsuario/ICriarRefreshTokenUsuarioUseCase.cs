using Wards.Application.UsesCases.Auths.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.CriarRefreshTokenUsuario
{
    public interface ICriarRefreshTokenUsuarioUseCase
    {
        Task<CriarRefreshTokenUsuarioOutput?> Execute(string token, string refreshToken, string email);
    }
}