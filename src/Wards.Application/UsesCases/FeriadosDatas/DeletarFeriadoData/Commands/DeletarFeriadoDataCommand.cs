using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData.Commands
{
    public class DeletarFeriadoDataCommand : IDeletarFeriadoDataCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public DeletarFeriadoDataCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(int feriadoId)
        {
            try
            {
                var fd = await _context.FeriadosDatas.
                Where(fe => fe.FeriadoId == feriadoId).ToListAsync();

                if (fd is not null)
                {
                    _context.FeriadosDatas.RemoveRange(fd);
                    await _context.SaveChangesAsync();
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