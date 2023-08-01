using Wards.Application.UseCases.Hashtags.Shared.Output;

namespace Wards.Application.UseCases.Hashtags.ListarHashtag
{
    public interface IListarHashtagUseCase
    {
        Task<IEnumerable<HashtagOutput>> Execute();
    }
}