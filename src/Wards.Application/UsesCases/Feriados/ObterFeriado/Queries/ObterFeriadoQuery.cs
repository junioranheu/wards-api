using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Wards.Application.UseCases.Feriados.ObterFeriado.Queries
{
    public class ObterFeriadoQuery : IObterFeriadoQuery
    {
        private readonly WardsContext _context;

        public ObterFeriadoQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<Feriado?> Execute(int id)
        {
            var linq = await _context.Feriados.
                             Include(u => u.Usuarios).
                             Include(um => um.UsuariosMods).
                             Include(fd => fd.FeriadosDatas).
                             Include(fe => fe.FeriadosEstados)!.ThenInclude(e => e.Estados).
                             Where(ct => ct.FeriadoId == id).
                             FirstOrDefaultAsync();

            return linq;
        }
    }
}