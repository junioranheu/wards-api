using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.CriarLog
{
    public interface ICriarLogUseCase
    {
        Task<int> Criar(Log input);
    }
}