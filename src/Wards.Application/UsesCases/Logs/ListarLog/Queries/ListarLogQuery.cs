using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Application.UsesCases.Shared.Models;
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

        public async Task<IEnumerable<Log>> Execute(PaginacaoInput input)
        {
            try
            {
                var linq = await _context.Logs.
                                 Include(u => u.Usuarios).
                                 Skip((input.IsSelectAll ? 0 : input.Pagina * input.Limit)).
                                 Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                                 OrderByDescending(l => l.LogId).
                                 AsNoTracking().ToListAsync();

                return linq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}