using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Tokens.DeletarRefreshToken.Commands
{
    public sealed class DeletarRefreshTokenCommand : IDeletarRefreshTokenCommand
    {
        private readonly WardsContext _context;

        public DeletarRefreshTokenCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(RefreshToken input)
        {
            var linq = await _context.RefreshTokens.
                       Where(u => u.UsuarioId == input.UsuarioId).
                       AsNoTracking().ToListAsync();

            if (linq.Any())
            {
                _context.RefreshTokens.RemoveRange(linq);
            }
        }
    }
}