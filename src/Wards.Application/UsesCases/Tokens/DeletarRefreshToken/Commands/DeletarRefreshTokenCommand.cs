using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands
{
    public sealed class DeletarRefreshTokenCommand : IDeletarRefreshTokenCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public DeletarRefreshTokenCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(RefreshToken input)
        {
            try
            {
                var linq = await _context.RefreshTokens.
                                 Where(u => u.UsuarioId == input.UsuarioId).
                                 AsNoTracking().ToListAsync();

                if (linq is not null)
                {
                    _context.RefreshTokens.RemoveRange(linq);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}