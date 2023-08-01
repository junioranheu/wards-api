using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Hashtags.ListarHashtag.Queries
{
    public interface IListarHashtagQuery
    {
        Task<IEnumerable<Hashtag>> Execute();
    }
}