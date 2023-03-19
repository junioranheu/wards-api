using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Wards.CriarWard.Commands
{
    public interface ICriarWardCommand
    {
        Task<int> Execute(Ward input);
    }
}