using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.WardsHashtags.Shared.Output;

namespace Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd.Queries
{
    public interface IListarHashtagQtdQuery
    {
        Task<IEnumerable<HashtagQtdOutput>> Execute(int max);
    }
}