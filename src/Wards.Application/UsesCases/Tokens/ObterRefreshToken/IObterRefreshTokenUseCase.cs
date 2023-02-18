namespace Wards.Application.UsesCases.Tokens.ObterRefreshToken
{
    public interface IObterRefreshTokenUseCase
    {
        Task<string> ObterByUsuarioId(int id);
    }
}