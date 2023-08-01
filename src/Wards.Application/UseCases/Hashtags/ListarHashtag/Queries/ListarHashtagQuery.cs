using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Hashtags.ListarHashtag.Queries
{
    public sealed class ListarHashtagQuery : IListarHashtagQuery
    {
        private readonly WardsContext _context;

        public ListarHashtagQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hashtag>> Execute()
        {
            var linq = await _context.Hashtags.
                       Where(h => h.IsAtivo == true).
                       AsNoTracking().ToListAsync();

            return linq;
        }
    }
}