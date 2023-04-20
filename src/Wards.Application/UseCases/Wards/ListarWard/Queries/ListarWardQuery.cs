using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Wards.ListarWard.Queries
{
    public sealed class ListarWardQuery : IListarWardQuery
    {
        private readonly WardsContext _context;

        public ListarWardQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ward>> Execute(PaginacaoInput input)
        {
            var linq = await _context.Wards.
                             Include(u => u.Usuarios).
                             Include(u => u.UsuariosMods).
                             Where(w => w.IsAtivo == true).
                             Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                             Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                             AsNoTracking().ToListAsync();

            return linq;
        }
    }
}