using Wards.Application.UseCases.WardsHashtags.Shared.Output;

namespace Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd
{
    public interface IListarHashtagQtdUseCase
    {
        Task<IEnumerable<HashtagQtdOutput>> Execute();
    }
}