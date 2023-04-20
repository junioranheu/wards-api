using Wards.Application.UseCases.Auths.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.CriarRefreshTokenUsuario
{
    public interface ICriarRefreshTokenUsuarioUseCase
    {
        Task<CriarRefreshTokenUsuarioOutput?> Execute(string token, string refreshToken, string email);
    }
}