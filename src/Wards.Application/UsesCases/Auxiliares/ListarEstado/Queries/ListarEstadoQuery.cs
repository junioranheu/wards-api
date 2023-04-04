using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries
{
    public sealed class ListarEstadoQuery : IListarEstadoQuery
    {
        private readonly WardsContext _context;

        public ListarEstadoQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Estado>> ExecuteAsync()
        {
            var linq = await _context.Estados.
                       AsNoTracking().ToListAsync();

            return linq;
        }
    }
}