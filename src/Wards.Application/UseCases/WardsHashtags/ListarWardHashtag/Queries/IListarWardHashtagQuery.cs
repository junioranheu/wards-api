using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.WardsHashtags.ListarWardHashtag.Queries
{
    public interface IListarWardHashtagQuery
    {
        Task<IEnumerable<WardHashtag>> Execute(PaginacaoInput input);
    }
}