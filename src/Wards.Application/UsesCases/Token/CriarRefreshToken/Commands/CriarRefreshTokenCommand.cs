using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Token.CriarRefreshToken.Commands
{
    public sealed class CriarRefreshTokenCommand
    {
        public readonly WardsContext _context;

        public CriarRefreshTokenCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task? Criar(RefreshToken input)
        {
            // #1 - Excluir refresh token, caso exista;
            var dados = await _context.RefreshTokens.Where(u => u.UsuarioId == input.UsuarioId).AsNoTracking().ToListAsync();

            if (dados is not null)
            {
                _context.RefreshTokens.RemoveRange(dados);
            }

            await _context.AddAsync(input);
            await _context.SaveChangesAsync();
        }
    }
}
