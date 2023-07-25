using AutoMapper;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Wards.ListarWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.UseCases.Wards.ListarWard
{
    public sealed class ListarWardUseCase : IListarWardUseCase
    {
        private readonly IMapper _map;
        private readonly IListarWardQuery _listarQuery;

        public ListarWardUseCase(IMapper map, IListarWardQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<WardOutput>> Execute(PaginacaoInput input)
        {
            var query = await _listarQuery.Execute(input);

            if (query is null)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado));
            }

            var output = _map.Map<IEnumerable<WardOutput>>(await _listarQuery.Execute(input));

            foreach (var item in output)
            {
                item.ListaHashtags = query.Where(x => x.WardId == item.WardId).FirstOrDefault()!.WardsHashtags!.Select(x => x.Hashtags!.Nome).ToArray();
            }

            return output;
        }
    }
}