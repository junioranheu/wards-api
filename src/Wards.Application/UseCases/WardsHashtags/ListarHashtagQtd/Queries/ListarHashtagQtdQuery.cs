using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.WardsHashtags.Shared.Output;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd.Queries
{
    public sealed class ListarHashtagQtdQuery : IListarHashtagQtdQuery
    {
        private readonly WardsContext _context;

        public ListarHashtagQtdQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HashtagQtdOutput>> Execute(int max)
        {
            var linq = await _context.WardsHashtags.
                       Include(ht => ht.Hashtags).
                       GroupBy(h => h.HashtagId).
                       Select(x => new HashtagQtdOutput
                       {
                           Tag = x.Select(x => x.Hashtags!.Tag).FirstOrDefault()!,
                           Quantidade = x.Count()
                       }).
                       OrderByDescending(h => h.Quantidade).ThenBy(h => h.Tag).
                       Take(max > 0 ? max : int.MaxValue).
                       AsNoTracking().ToListAsync();

            return linq;
        }
    }
}