using Microsoft.EntityFrameworkCore;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Wards.DeletarWard.Commands
{
    public sealed class DeletarWardCommand : IDeletarWardCommand
    {
        private readonly WardsContext _context;

        public DeletarWardCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<bool> Execute(int id)
        {
            var linq = await _context.Wards.
                             Where(w => w.WardId == id).
                             AsNoTracking().FirstOrDefaultAsync();

            if (linq is null)
            {
                return false;
            }

            _context.Wards.Remove(linq);

            return true;
        }
    }
}