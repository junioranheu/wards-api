using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed;

namespace Wards.Application.Services.Sistemas.ResetarBancoDados.Commands
{
    public sealed class MigrateDatabaseCommand : IMigrateDatabaseCommand
    {
        private readonly WardsContext _context;

        public MigrateDatabaseCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(bool isAplicarMigrations)
        {
            await DbInitializer.Initialize(_context, isAplicarMigrations: isAplicarMigrations, isResetar: false);
        }
    }
}