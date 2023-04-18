using Microsoft.Extensions.Logging;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Feriados.CriarFeriado.Commands
{
    public sealed class CriarFeriadoCommand : ICriarFeriadoCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public CriarFeriadoCommand(WardsContext context, ILogger<CriarFeriadoCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Execute(Feriado input)
        {
            try
            {
                await _context.AddAsync(input);
                await _context.SaveChangesAsync();

                return input.FeriadoId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}