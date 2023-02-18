namespace Wards.Application.UsesCases.Tokens.ObterRefreshToken.Queries
{
    public interface IObterRefreshTokenQuery
    {
        Task<string> ObterByUsuarioId(int id);
    }
}