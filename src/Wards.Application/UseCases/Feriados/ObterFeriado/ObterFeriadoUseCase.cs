using AutoMapper;
using Wards.Application.UseCases.Feriados.ObterFeriado.Queries;
using Wards.Application.UseCases.Feriados.Shared.Models.Output;

namespace Wards.Application.UseCases.Feriados.ObterFeriado
{
    public class ObterFeriadoUseCase : IObterFeriadoUseCase
    {
        private readonly IMapper _map;
        private readonly IObterFeriadoQuery _obterCurvaTipicaQuery;

        public ObterFeriadoUseCase(IMapper map, IObterFeriadoQuery obterCurvaTipicaQuery)
        {
            _map = map;
            _obterCurvaTipicaQuery = obterCurvaTipicaQuery;
        }

        public async Task<FeriadoOutput> Execute(int id)
        {
            return _map.Map<FeriadoOutput>(await _obterCurvaTipicaQuery.Execute(id));
        }
    }
}
