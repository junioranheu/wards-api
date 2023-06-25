using AutoMapper;
using Wards.Application.UseCases.Auxiliares.ListarEstado.Queries;
using Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.Auxiliares.ListarEstado
{
    public sealed class ListarEstadoUseCase : IListarEstadoUseCase
    {
        private readonly IMapper _map;
        private readonly IListarEstadoQuery _listarEstadoQuery;

        public ListarEstadoUseCase(IMapper map, IListarEstadoQuery listarEstadoQuery)
        {
            _map = map;
            _listarEstadoQuery = listarEstadoQuery;
        }

        public async Task<IEnumerable<EstadoOutput>> Execute(PaginacaoInput input)
        {
            return _map.Map<IEnumerable<EstadoOutput>>(await _listarEstadoQuery.Execute(input));
        }
    }
}