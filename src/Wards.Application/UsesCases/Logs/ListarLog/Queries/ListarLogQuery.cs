using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Logs.ListarLog.Queries
{
    public sealed class ListarLogQuery : IListarLogQuery
    {
        private readonly WardsContext _context;

        public ListarLogQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Log>> Execute(int pagina, int tamanhoPagina)
        {
            var linq = await _context.Logs.
                             Include(u => u.Usuarios).
                             Skip(pagina * tamanhoPagina).Take(tamanhoPagina).
                             AsNoTracking().ToListAsync();

            return linq;
        }
    }
}