using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.FeriadosEstados.CriarFeriadoEstado.Commands
{
    public class CriarFeriadoEstadoCommand : ICriarFeriadoEstadoCommand
    {
        private readonly WardsContext _context;

        public CriarFeriadoEstadoCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(int[] estadoId, int feriadoId)
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
    }
}