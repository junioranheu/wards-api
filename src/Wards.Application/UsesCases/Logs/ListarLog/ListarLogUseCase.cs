using AutoMapper;
using Wards.Application.UsesCases.Logs.ListarLog.Queries;
using Wards.Application.UsesCases.Logs.Shared.Output;

namespace Wards.Application.UsesCases.Logs.ListarLog
{
    public sealed class ListarLogUseCase : IListarLogUseCase
    {
        private readonly IMapper _map;
        private readonly IListarLogQuery _listarQuery;

        public ListarLogUseCase(IMapper map, IListarLogQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<LogOutput>?> Execute(int pagina, int tamanhoPagina)
        {
            return _map.Map<IEnumerable<LogOutput>>(await _listarQuery.Execute(pagina, tamanhoPagina));
        }
    }
}