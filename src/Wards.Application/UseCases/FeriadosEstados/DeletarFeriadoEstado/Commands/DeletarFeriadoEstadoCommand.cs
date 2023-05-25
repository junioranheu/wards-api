using Microsoft.EntityFrameworkCore;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado.Commands
{
    public class DeletarFeriadoEstadoCommand : IDeletarFeriadoEstadoCommand
    {
        private readonly WardsContext _context;

        public DeletarFeriadoEstadoCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(int feriadoId)
        {
            var fe = await _context.FeriadosEstados.
                           Where(fe => fe.FeriadoId == feriadoId).ToListAsync();

            if (fe.Any())
            {
                _context.FeriadosEstados.RemoveRange(fe);
                await _context.SaveChangesAsync();
            }
        }
    }
}