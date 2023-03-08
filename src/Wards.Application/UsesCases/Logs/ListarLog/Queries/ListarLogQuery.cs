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

        public async Task<IEnumerable<Log>> Execute()
        {
            var linq = await _context.Logs.
                             Include(u => u.Usuarios).
                             AsNoTracking().ToListAsync();

            foreach (var item in linq)
            {
                if (item.Usuarios is not null)
                {
                    item.Usuarios.Senha = string.Empty;
                }
            }

            return linq;
        }
    }
}
