using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.AtualizarWard.Commands
{
    public interface IAtualizarWardCommand
    {
        Task<int> Execute(Ward input);
    }
}