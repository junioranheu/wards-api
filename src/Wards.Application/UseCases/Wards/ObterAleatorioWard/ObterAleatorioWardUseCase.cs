using AutoMapper;
using Wards.Application.UseCases.Wards.ObterAleatorioWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;

namespace Wards.Application.UseCases.Wards.ObterAleatorioWard
{
    public sealed class ObterAleatorioWardUseCase : IObterAleatorioWardUseCase
    {
        private readonly IMapper _map;
        private readonly IObterAleatorioWardQuery _obterAleatorioQuery;

        public ObterAleatorioWardUseCase(IMapper map, IObterAleatorioWardQuery obterAleatorioQuery)
        {
            _map = map;
            _obterAleatorioQuery = obterAleatorioQuery;
        }

        public async Task<WardOutput?> Execute()
        {
            return _map.Map<WardOutput>(await _obterAleatorioQuery.Execute());
        }
    }
}