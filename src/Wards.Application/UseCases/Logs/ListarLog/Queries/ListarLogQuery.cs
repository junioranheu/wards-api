using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Logs.ListarLog.Queries
{
    public sealed class ListarLogQuery : IListarLogQuery
    {
        private readonly WardsContext _context;

        public ListarLogQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Log>> Execute(PaginacaoInput input)
        {
            var linq = await _context.Logs.
                             Include(u => u.Usuarios).
                             OrderByDescending(l => l.LogId).
                             Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                             Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                             AsNoTracking().ToListAsync();

            return linq;
        }
    }
}