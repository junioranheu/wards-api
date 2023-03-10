using AutoMapper;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria
{
    public sealed class ObterUsuarioCondicaoArbitrariaUseCase : IObterUsuarioCondicaoArbitrariaUseCase
    {
        private readonly IMapper _map;
        private readonly IObterUsuarioCondicaoArbitrariaQuery _obterQuery;

        public ObterUsuarioCondicaoArbitrariaUseCase(IMapper map, IObterUsuarioCondicaoArbitrariaQuery obterQuery)
        {
            _map = map;
            _obterQuery = obterQuery;
        }

        public async Task<(UsuarioOutput?, string)> Execute(string? email, string? nomeUsuarioSistema)
        {
            (Usuario, string) resp = await _obterQuery.Execute(email, nomeUsuarioSistema);
            return (_map.Map<UsuarioOutput>(resp.Item1), resp.Item2);
        }
    }
}