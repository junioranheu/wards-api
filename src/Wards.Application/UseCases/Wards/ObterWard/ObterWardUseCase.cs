using AutoMapper;
using Wards.Application.UseCases.Wards.ObterWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.UseCases.Wards.ObterWard
{
    public sealed class ObterWardUseCase : IObterWardUseCase
    {
        private readonly IMapper _map;
        private readonly IObterWardQuery _obterQuery;

        public ObterWardUseCase(IMapper map, IObterWardQuery obterQuery)
        {
            _map = map;
            _obterQuery = obterQuery;
        }

        public async Task<WardOutput?> Execute(int id)
        {
            var query = await _obterQuery.Execute(id);

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