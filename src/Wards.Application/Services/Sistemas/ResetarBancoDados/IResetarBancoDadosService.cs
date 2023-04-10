namespace Wards.Application.Services.Sistemas.ResetarBancoDados
{
    public interface IResetarBancoDadosService
    {
        Task<bool> Execute();
    }
}