namespace Wards.Application.UsesCases.Logs.CriarLog.Commands
{
    public interface ICriarLogCommand
    {
        Task<int> ExecuteAsync(Log dto);
    }
}