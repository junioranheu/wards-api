using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria
{
    public sealed class ObterUsuarioCondicaoArbitrariaUseCase : IObterUsuarioCondicaoArbitrariaUseCase
    {
        private readonly IObterUsuarioCondicaoArbitrariaQuery _obterQuery;

        public ObterUsuarioCondicaoArbitrariaUseCase(IObterUsuarioCondicaoArbitrariaQuery obterQuery)
        {
            _obterQuery = obterQuery;
        }

        public async Task<Usuario> Execute(string? email, string? nomeUsuarioSistema)
        {
            return await _obterQuery.Execute(email, nomeUsuarioSistema);
        }
    }
}
