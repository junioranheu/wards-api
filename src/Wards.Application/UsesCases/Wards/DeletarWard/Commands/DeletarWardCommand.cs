using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Wards.DeletarWard.Commands
{
    public sealed class DeletarWardCommand : IDeletarWardCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public DeletarWardCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Execute(int id)
        {
            try
            {
                var linq = await _context.Wards.
                                 Where(w => w.WardId == id).
                                 AsNoTracking().FirstOrDefaultAsync();

                if (linq is null)
                {
                    return false;
                }

                _context.Wards.Remove(linq);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}