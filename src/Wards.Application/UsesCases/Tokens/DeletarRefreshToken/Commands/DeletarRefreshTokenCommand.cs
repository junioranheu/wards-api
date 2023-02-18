using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands
{
    public sealed class DeletarRefreshTokenCommand : IDeletarRefreshTokenCommand
    {
        public readonly WardsContext _context;

        public DeletarRefreshTokenCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<bool> Deletar(RefreshToken input)
        {
            var dados = await _context.RefreshTokens.Where(u => u.UsuarioId == input.UsuarioId).AsNoTracking().ToListAsync();

            if (dados is not null)
            {
                _context.RefreshTokens.RemoveRange(dados);
            }

            return true;
        }
    }
}
