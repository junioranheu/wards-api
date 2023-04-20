namespace Wards.Application.UseCases.Tokens.ObterRefreshToken.Queries
{
    public interface IObterRefreshTokenQuery
    {
        Task<string> Execute(int id);
    }
}