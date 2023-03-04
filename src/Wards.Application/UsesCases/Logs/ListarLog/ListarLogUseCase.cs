using Wards.Application.UsesCases.Logs.ListarLog.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.ListarLog
{
    public sealed class ListarLogUseCase : IListarLogUseCase
    {
        private readonly IListarLogQuery _listarQuery;

        public ListarLogUseCase(IListarLogQuery listarQuery)
        {
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<Log>?> Listar()
        {
            return await _listarQuery.Listar();
        }
    }
}
