using AutoMapper;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.WardsHashtags.ListarWardHashtag.Queries;
using Wards.Application.UseCases.WardsHashtags.Shared.Output;

namespace Wards.Application.UseCases.WardsHashtags.ListarWardHashtag
{
    public sealed class ListarWardHashtagUseCase : IListarWardHashtagUseCase
    {
        private readonly IMapper _map;
        private readonly IListarWardHashtagQuery _listarQuery;

        public ListarWardHashtagUseCase(IMapper map, IListarWardHashtagQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<WardHashtagOutput>> Execute(PaginacaoInput input)
        {
            return _map.Map<IEnumerable<WardHashtagOutput>>(await _listarQuery.Execute(input));
        }
    }
}