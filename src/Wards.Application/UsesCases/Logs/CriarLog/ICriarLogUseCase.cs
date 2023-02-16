namespace Wards.Application.UsesCases.Logs.CriarLog
{
    public interface ICriarLogUseCase
    {
        Task<int> ExecuteAsync(Log dto);
    }
}