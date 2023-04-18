using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Application.UsesCases.Shared.Models;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Wards.ListarWard.Queries
{
    public sealed class ListarWardQuery : IListarWardQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ListarWardQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Ward>> Execute(PaginacaoInput input)
        {
            try
            {
                var linq = await _context.Wards.
                                 Include(u => u.Usuarios).
                                 Include(u => u.UsuariosMods).
                                 Skip((input.IsSelectAll ? 0 : input.Pagina * input.Limit)).
                                 Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
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