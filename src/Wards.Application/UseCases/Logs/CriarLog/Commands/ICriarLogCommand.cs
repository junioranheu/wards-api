using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Logs.CriarLog.Commands
{
    public interface ICriarLogCommand
    {
        Task Execute(Log input);
    }
}