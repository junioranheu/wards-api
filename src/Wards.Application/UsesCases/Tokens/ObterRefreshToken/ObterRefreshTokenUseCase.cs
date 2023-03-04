using Wards.Application.UsesCases.Tokens.ObterRefreshToken.Queries;

namespace Wards.Application.UsesCases.Tokens.ObterRefreshToken
{
    public sealed class ObterRefreshTokenUseCase : IObterRefreshTokenUseCase
    {
        private readonly IObterRefreshTokenQuery _obterQuery;

        public ObterRefreshTokenUseCase(IObterRefreshTokenQuery obterQuery)
        {
            _obterQuery = obterQuery;
        }

        public async Task<string> Execute(int id)
        {
            return await _obterQuery.Execute(id);
        }
    }
}
