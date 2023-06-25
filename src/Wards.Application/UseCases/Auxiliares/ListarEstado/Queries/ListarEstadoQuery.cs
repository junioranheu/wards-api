using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Auxiliares.ListarEstado.Queries
{
    public sealed class ListarEstadoQuery : IListarEstadoQuery
    {
        private readonly WardsContext _context;

        public ListarEstadoQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Estado>> Execute(PaginacaoInput input)
        {
            var linq = await _context.Estados.
                       Where(e => e.IsAtivo == true).
                       OrderBy(e => e.EstadoId).
                       Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                       Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                       AsNoTracking().ToListAsync();

            return linq;
        }
    }
}