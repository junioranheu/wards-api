using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.WardsHashtags.Shared.Output;

namespace Wards.Application.UseCases.WardsHashtags.ListarWardHashtag
{
    public interface IListarWardHashtagUseCase
    {
        Task<IEnumerable<WardHashtagOutput>> Execute(PaginacaoInput input);
    }
}