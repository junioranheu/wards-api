using AutoMapper;
using Wards.Application.UseCases.Wards.ObterWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;

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
            return _map.Map<WardOutput>(await _obterQuery.Execute(id));
        }
    }
}