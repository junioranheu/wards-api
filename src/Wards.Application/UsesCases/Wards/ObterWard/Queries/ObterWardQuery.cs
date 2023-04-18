using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Wards.ObterWard.Queries
{
    public sealed class ObterWardQuery : IObterWardQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ObterWardQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Ward?> Execute(int id)
        {
            try
            {
                var linq = await _context.Wards.
                                 Include(u => u.Usuarios).
                                 Include(u => u.UsuariosMods).
                                 Where(w => w.WardId == id && w.IsAtivo == true).
                                 AsNoTracking().FirstOrDefaultAsync();

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