using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.ObterWard.Queries
{
    public interface IObterWardQuery
    {
        Task<Ward?> Execute(int id);
    }
}