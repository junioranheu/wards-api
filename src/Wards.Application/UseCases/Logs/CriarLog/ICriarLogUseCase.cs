using Wards.Application.UseCases.Logs.Shared.Input;

namespace Wards.Application.UseCases.Logs.CriarLog
{
    public interface ICriarLogUseCase
    {
        Task Execute(LogInput input);
    }
}