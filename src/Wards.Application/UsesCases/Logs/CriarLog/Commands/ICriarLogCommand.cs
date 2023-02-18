using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.CriarLog.Commands
{
    public interface ICriarLogCommand
    {
        Task<int> Criar(Log input);
    }
}