using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Logs.CriarLog.Commands
{
    public sealed class CriarLogCommand : ICriarLogCommand
    {
        private readonly WardsContext _context;

        public CriarLogCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(Log input)
        {
            _context.ChangeTracker.Clear();

            await _context.AddAsync(input);
            await _context.SaveChangesAsync();
        }
    }
}