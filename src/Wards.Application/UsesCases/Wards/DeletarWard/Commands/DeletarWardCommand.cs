using Microsoft.EntityFrameworkCore;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Wards.DeletarWard.Commands
{
    public sealed class DeletarWardCommand : IDeletarWardCommand
    {
        private readonly WardsContext _context;

        public DeletarWardCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(int id)
        {
            var linq = await _context.Wards.
                             Where(w => w.WardId == id).
                             AsNoTracking().FirstOrDefaultAsync();

            if (linq is not null)
            {
                _context.Wards.Remove(linq);
            }
        }
    }
}