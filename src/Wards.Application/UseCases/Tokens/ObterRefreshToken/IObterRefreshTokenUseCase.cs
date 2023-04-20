namespace Wards.Application.UseCases.Tokens.ObterRefreshToken
{
    public interface IObterRefreshTokenUseCase
    {
        Task<string> Execute(int id);
    }
}