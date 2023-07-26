using AutoMapper;
using Wards.Application.UseCases.Wards.ObterAleatorioWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

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
            var query = await _obterAleatorioQuery.Execute();

            if (query is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            var output = _map.Map<WardOutput>(query);
            output.ListaHashtags = query!.WardsHashtags!.Select(x => x.Hashtags!.Tag).ToArray();

            return output;
        }
    }
}