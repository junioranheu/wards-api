using Wards.Application.UsesCases.Logs.Shared.Input;

namespace Wards.Application.UsesCases.Logs.CriarLog
{
    public interface ICriarLogUseCase
    {
        Task Execute(LogInput input);
    }
}