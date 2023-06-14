namespace Wards.Application.Services.Sistemas.ResetarBancoDados
{
    public interface IMigrateDatabaseService
    {
        Task Execute(bool isAplicarMigrations);
    }
}