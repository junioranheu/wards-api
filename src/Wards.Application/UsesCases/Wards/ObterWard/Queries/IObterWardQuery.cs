using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Wards.ObterWard.Queries
{
    public interface IObterWardQuery
    {
        Task<Ward?> Execute(int id);
    }
}