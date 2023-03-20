using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Wards.ListarWard.Queries
{
    public sealed class ListarWardQuery : IListarWardQuery
    {
        private readonly WardsContext _context;

        public ListarWardQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ward>> Execute()
        {
            var linq = await _context.Wards.
                             Include(u => u.Usuarios).
                             AsNoTracking().ToListAsync();

            return linq;
        }
    }
}