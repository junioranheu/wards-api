using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Feriados.ObterFeriado.Queries
{
    public class ObterFeriadoQuery : IObterFeriadoQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ObterFeriadoQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Feriado?> Execute(int id)
        {
            try
            {
                var linq = await _context.Feriados.
                 Include(u => u.Usuarios).
                 Include(um => um.UsuariosMods).
                 Include(fd => fd.FeriadosDatas).
                 Include(fe => fe.FeriadosEstados)!.ThenInclude(e => e.Estados).
                 Where(ct => ct.FeriadoId == id).
                 FirstOrDefaultAsync();

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