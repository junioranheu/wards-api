using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Wards.ObterWard.Queries
{
    public sealed class ObterWardQuery : IObterWardQuery
    {
        private readonly WardsContext _context;

        public ObterWardQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<Ward?> Execute(int id)
        {
            var linq = await _context.Wards.
                             Include(u => u.Usuarios).
                             Include(u => u.UsuariosMods).
                             Where(w => w.WardId == id).
                             AsNoTracking().FirstOrDefaultAsync();

            return linq;
        }
    }
}