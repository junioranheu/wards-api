using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries
{
    public sealed class ListarEstadoQuery : IListarEstadoQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ListarEstadoQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Estado>> Execute()
        {
            try
            {
                var linq = await _context.Estados.
                           Where(e => e.IsAtivo == true).
                           AsNoTracking().ToListAsync();

                return linq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}