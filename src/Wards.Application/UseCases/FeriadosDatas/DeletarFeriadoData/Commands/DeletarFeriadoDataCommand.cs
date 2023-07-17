using Microsoft.EntityFrameworkCore;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData.Commands
{
    public class DeletarFeriadoDataCommand : IDeletarFeriadoDataCommand
    {
        private readonly WardsContext _context;

        public DeletarFeriadoDataCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(int feriadoId)
        {
            var fd = await _context.FeriadosDatas.
                     Where(fe => fe.FeriadoId == feriadoId).ToListAsync();

            if (fd.Any())
            {
                _context.FeriadosDatas.RemoveRange(fd);
                await _context.SaveChangesAsync();
            }
        }
    }
}