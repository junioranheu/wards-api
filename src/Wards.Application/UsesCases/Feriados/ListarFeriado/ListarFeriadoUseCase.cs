using AutoMapper;
using Wards.Application.UseCases.Feriados.ListarFeriado.Queries;
using Wards.Application.UsesCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UseCases.Feriados.ListarFeriado
{
    public class ListarFeriadoUseCase : IListarFeriadoUseCase
    {
        private readonly IMapper _map;
        private readonly IListarFeriadoQuery _listarFeriadoQuery;

        public ListarFeriadoUseCase(IMapper map, IListarFeriadoQuery listarFeriadoQuery)
        {
            _map = map;
            _listarFeriadoQuery = listarFeriadoQuery;
        }

        public async Task<IEnumerable<FeriadoOutput>> Execute()
        {
            return _map.Map<IEnumerable<FeriadoOutput>>(await _listarFeriadoQuery.Execute());
        }
    }
}