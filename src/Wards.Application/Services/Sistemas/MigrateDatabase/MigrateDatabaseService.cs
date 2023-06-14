using Wards.Application.Services.Sistemas.ResetarBancoDados.Commands;

namespace Wards.Application.Services.Sistemas.ResetarBancoDados
{
    public sealed class MigrateDatabaseService : IMigrateDatabaseService
    {
        private readonly IMigrateDatabaseCommand _migrateDatabaseCommand;

        public MigrateDatabaseService(IMigrateDatabaseCommand migrateDatabaseCommand)
        {
            _migrateDatabaseCommand = migrateDatabaseCommand;
        }

        public async Task Execute(bool isAplicarMigrations)
        {
            await _migrateDatabaseCommand.Execute(isAplicarMigrations);
        }
    }
}