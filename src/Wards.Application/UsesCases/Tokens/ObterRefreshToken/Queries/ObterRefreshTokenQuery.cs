using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Tokens.ObterRefreshToken.Queries
{
    public sealed class ObterRefreshTokenQuery : IObterRefreshTokenQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ObterRefreshTokenQuery(WardsContext context, ILogger<ObterRefreshTokenQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> Execute(int id)
        {
            try
            {
                // É necessário verificar se o refresh token com base no id do usuário e se o usuário de fato está ativo...
                // Isso para ajudar numa possível "black-list";
                var linq = await _context.RefreshTokens.
                                 Include(u => u.Usuarios).
                                 Where(r => r.UsuarioId == id && r.Usuarios!.IsAtivo == true).
                                 AsNoTracking().FirstOrDefaultAsync();

                return linq?.RefToken ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}