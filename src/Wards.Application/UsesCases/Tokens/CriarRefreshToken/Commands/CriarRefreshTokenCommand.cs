using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands
{
    public sealed class CriarRefreshTokenCommand : ICriarRefreshTokenCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public CriarRefreshTokenCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(RefreshToken input)
        {
            try
            {
                await _context.AddAsync(input);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}
