using Microsoft.EntityFrameworkCore;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Tokens.ObterRefreshToken.Queries
{
    public sealed class ObterRefreshTokenQuery : IObterRefreshTokenQuery
    {
        public readonly WardsContext _context;

        public ObterRefreshTokenQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<string> ObterByUsuarioId(int id)
        {
            // É necessário verificar se o refresh token com base no id do usuário e se o usuário de fato está ativo...
            // Isso para ajudar numa possível "black-list";
            var byId = await _context.RefreshTokens.
                       Include(u => u.Usuarios).
                       Where(r => r.UsuarioId == id && r.Usuarios.IsAtivo == true).
                       AsNoTracking().FirstOrDefaultAsync();

            return byId?.RefToken ?? string.Empty;
        }
    }
}
