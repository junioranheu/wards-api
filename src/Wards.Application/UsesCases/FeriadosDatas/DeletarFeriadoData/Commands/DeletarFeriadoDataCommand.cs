using Wards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData.Commands
{
    public class DeletarFeriadoDataCommand : IDeletarFeriadoDataCommand
    {
        private readonly WardsContext _context;

        public DeletarFeriadoDataCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(int feriadoId)
        {
            var fd = await _context.FeriadosDatas.
                           Where(fe => fe.FeriadoId == feriadoId).ToListAsync();

            if (fd is not null)
            {
                _context.FeriadosDatas.RemoveRange(fd);
                await _context.SaveChangesAsync();
            }
        }
    }
}