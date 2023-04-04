using Wards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado.Commands
{
    public class DeletarFeriadoEstadoCommand : IDeletarFeriadoEstadoCommand
    {
        private readonly WardsContext _context;

        public DeletarFeriadoEstadoCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(int feriadoId)
        {
            var fe = await _context.FeriadosEstados.
                           Where(fe => fe.FeriadoId == feriadoId).ToListAsync();

            if (fe is not null)
            {
                _context.FeriadosEstados.RemoveRange(fe);
                await _context.SaveChangesAsync();
            }
        }
    }
}