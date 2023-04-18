using Wards.Application.UsesCases.Shared.Models;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Wards.ListarWard.Queries
{
    public interface IListarWardQuery
    {
        Task<IEnumerable<Ward>> Execute(PaginacaoInput input);
    }
}