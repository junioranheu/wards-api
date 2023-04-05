namespace Wards.Application.Services.Sistemas.ResetarBancoDados.Commands
{
    public interface IResetarBancoDadosCommand
    {
        Task<bool> ExecuteAsync();
    }
}