using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado.Commands
{
    public class CriarFeriadoEstadoCommand : ICriarFeriadoEstadoCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public CriarFeriadoEstadoCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(int[] estadoId, int feriadoId)
        {
            try
            {
                List<FeriadoEstado> listFe = new();

                for (int i = 0; i < estadoId?.Length; i++)
                {
                    FeriadoEstado fe = new()
                    {
                        FeriadoId = feriadoId,
                        EstadoId = estadoId[i]
                    };

                    listFe.Add(fe);
                }

                await _context.AddRangeAsync(listFe);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}