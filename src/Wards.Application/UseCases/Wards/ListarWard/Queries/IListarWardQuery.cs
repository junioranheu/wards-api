using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.ListarWard.Queries
{
    public interface IListarWardQuery
    {
        Task<IEnumerable<Ward>> Execute(PaginacaoInput input, string keyword);
    }
}