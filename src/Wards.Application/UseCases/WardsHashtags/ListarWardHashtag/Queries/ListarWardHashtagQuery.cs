using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.WardsHashtags.ListarWardHashtag.Queries
{
    public sealed class ListarWardHashtagQuery : IListarWardHashtagQuery
    {
        private readonly WardsContext _context;

        public ListarWardHashtagQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WardHashtag>> Execute(PaginacaoInput input)
        {
            var linq = await _context.WardsHashtags.
                       Include(ht => ht.Hashtags).
                       Where(wh => wh.IsAtivo == true).
                       Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                       Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                       AsNoTracking().ToListAsync();

            var aea = await _context.Hashtags.
                      GroupBy(h => h.HashtagId).
                      Select(group => new
                      {
                           HashtagId = group.Key,
                           Count = group.Count()
                      }).AsNoTracking().ToListAsync();

            return linq;
        }
    }
}