using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands
{
    public sealed class CriarRefreshTokenCommand : ICriarRefreshTokenCommand
    {
        private readonly WardsContext _context;

        public CriarRefreshTokenCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(RefreshToken input)
        {
            await _context.AddAsync(input);
            await _context.SaveChangesAsync();
        }
    }
}
