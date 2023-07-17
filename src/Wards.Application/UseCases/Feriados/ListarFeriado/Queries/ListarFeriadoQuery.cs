using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
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

        public async Task<IEnumerable<Feriado>> Execute(PaginacaoInput input)
        {
            var linq = await _context.Feriados.
                       Include(u => u.Usuarios).
                       Include(um => um.UsuariosMods).
                       Include(fd => fd.FeriadosDatas).
                       Include(fe => fe.FeriadosEstados)!.ThenInclude(e => e.Estados).                              
                       Where(f => f.IsAtivo == true).
                       OrderBy(f => f.FeriadoId).
                       Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                       Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                       ToListAsync();

            return linq;
        }
    }
}