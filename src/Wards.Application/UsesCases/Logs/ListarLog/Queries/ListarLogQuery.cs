using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Logs.ListarLog.Queries
{
    public sealed class ListarLogQuery : IListarLogQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ListarLogQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Log>> Execute(int pagina, int tamanhoPagina)
        {
            try
            {
                var linq = await _context.Logs.
                                 Include(u => u.Usuarios).
                                 Skip(pagina * tamanhoPagina).Take(tamanhoPagina).
                                 OrderByDescending(l => l.LogId).
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