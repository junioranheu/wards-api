namespace Wards.Application.Services.Sistemas.ResetarBancoDados.Commands
{
    public interface IMigrateDatabaseCommand
    {
        Task Execute(bool isAplicarMigrations);
    }
}