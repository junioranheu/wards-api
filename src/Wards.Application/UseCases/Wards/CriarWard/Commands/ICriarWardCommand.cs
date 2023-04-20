using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.CriarWard.Commands
{
    public interface ICriarWardCommand
    {
        Task<int> Execute(Ward input);
    }
}