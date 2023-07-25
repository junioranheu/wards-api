using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands
{
    public sealed class CriarWardHashtagCommand : ICriarWardHashtagCommand
    {
        private readonly WardsContext _context;

        public CriarWardHashtagCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(List<WardHashtag> listaInput, int wardId)
        {
            var dadosAntigos = await _context.WardsHashtags.
                               Where(wh => wh.WardId == wardId).
                               AsNoTracking().ToListAsync();

            if (dadosAntigos.Any())
            {
                _context.ChangeTracker.Clear();
                _context.WardsHashtags.RemoveRange(dadosAntigos);
                await _context.SaveChangesAsync();
            }

            await _context.AddRangeAsync(listaInput);
            await _context.SaveChangesAsync();
        }
    }
}