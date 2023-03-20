using AutoMapper;
using Wards.Application.UsesCases.Wards.ListarWard.Queries;
using Wards.Application.UsesCases.Wards.Shared.Output;

namespace Wards.Application.UsesCases.Wards.ListarWard
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

        public async Task<IEnumerable<WardOutput>?> Execute()
        {
            return _map.Map<IEnumerable<WardOutput>>(await _listarQuery.Execute());
        }
    }
}