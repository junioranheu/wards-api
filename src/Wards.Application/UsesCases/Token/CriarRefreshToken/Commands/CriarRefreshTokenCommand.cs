using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Token.CriarRefreshToken.Commands
{
    public sealed class CriarRefreshTokenCommand
    {
        public async Task? Criar(RefreshToken input)
        {
            // #1 - Excluir refresh token, caso exista;
            var dados = await _context.RefreshTokens.Where(u => u.UsuarioId == dto.UsuarioId).AsNoTracking().ToListAsync();

            if (dados is not null)
            {
                _context.RefreshTokens.RemoveRange(dados);
            }

            // #2 - Adicionar novo refresh token;
            RefreshToken item = _map.Map<RefreshToken>(dto);

            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
        }
    }
}
