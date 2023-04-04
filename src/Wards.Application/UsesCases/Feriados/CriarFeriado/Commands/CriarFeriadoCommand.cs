using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Feriados.CriarFeriado.Commands
{
    public sealed class CriarFeriadoCommand : ICriarFeriadoCommand
    {
        private readonly WardsContext _context;

        public CriarFeriadoCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<int> ExecuteAsync(Feriado input)
        {
            await _context.AddAsync(input);
            await _context.SaveChangesAsync();

            return input.FeriadoId;
        }
    }
}