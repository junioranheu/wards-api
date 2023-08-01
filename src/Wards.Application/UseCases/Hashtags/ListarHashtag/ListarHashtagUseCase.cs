using AutoMapper;
using Wards.Application.UseCases.Hashtags.ListarHashtag.Queries;
using Wards.Application.UseCases.Hashtags.Shared.Output;

namespace Wards.Application.UseCases.Hashtags.ListarHashtag
{
    public sealed class ListarHashtagUseCase : IListarHashtagUseCase
    {
        private readonly IMapper _map;
        private readonly IListarHashtagQuery _listarQuery;

        public ListarHashtagUseCase(IMapper map, IListarHashtagQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<HashtagOutput>> Execute()
        {
            return _map.Map<IEnumerable<HashtagOutput>>(await _listarQuery.Execute());
        }
    }
}