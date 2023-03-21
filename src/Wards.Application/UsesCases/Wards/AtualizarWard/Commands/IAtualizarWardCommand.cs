using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Wards.AtualizarWard.Commands
{
    public interface IAtualizarWardCommand
    {
        Task<int> Execute(Ward input);
    }
}