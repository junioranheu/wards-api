using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Feriados.ListarFeriado.Queries
{
    public class ListarFeriadoQuery : IListarFeriadoQuery
    {
        private readonly WardsContext _context;

        public ListarFeriadoQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feriado>> Execute()
        {
            var linq = await _context.Feriados.
                              Include(u => u.Usuarios).
                              Include(um => um.UsuariosMods).
                              Include(fd => fd.FeriadosDatas).
                              Include(fe => fe.FeriadosEstados)!.ThenInclude(e => e.Estados).
                              ToListAsync();

            return linq;
        }
    }
}