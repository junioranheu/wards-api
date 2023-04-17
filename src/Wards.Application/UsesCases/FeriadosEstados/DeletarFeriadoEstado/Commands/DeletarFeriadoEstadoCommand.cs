using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.FeriadosEstados.DeletarFeriadoEstado.Commands
{
    public class DeletarFeriadoEstadoCommand : IDeletarFeriadoEstadoCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public DeletarFeriadoEstadoCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(int feriadoId)
        {
            try
            {
                var fe = await _context.FeriadosEstados.
                               Where(fe => fe.FeriadoId == feriadoId).ToListAsync();

                if (fe is not null)
                {
                    _context.FeriadosEstados.RemoveRange(fe);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}