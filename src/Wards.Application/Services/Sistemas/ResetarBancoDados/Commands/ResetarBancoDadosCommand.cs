using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed;

namespace Wards.Application.Services.Sistemas.ResetarBancoDados.Commands
{
    public sealed class ResetarBancoDadosCommand : IResetarBancoDadosCommand
    {
        private readonly WardsContext _context;

        public ResetarBancoDadosCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                await DbInitializer.Initialize(_context);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}