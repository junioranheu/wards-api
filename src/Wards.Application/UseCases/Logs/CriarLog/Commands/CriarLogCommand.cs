using Microsoft.Extensions.Logging;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Logs.CriarLog.Commands
{
    public sealed class CriarLogCommand : ICriarLogCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public CriarLogCommand(WardsContext context, ILogger<CriarLogCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(Log input)
        {
            try
            {
                _context.ChangeTracker.Clear();

                await _context.AddAsync(input);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Houve uma falha ao registrar log na base de dados");
            }
        }
    }
}
