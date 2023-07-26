using Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd.Queries;
using Wards.Application.UseCases.WardsHashtags.Shared.Output;

namespace Wards.Application.UseCases.WardsHashtags.ListarHashtagQtd
{
    public sealed class ListarHashtagQtdUseCase : IListarHashtagQtdUseCase
    {
        private readonly IListarHashtagQtdQuery _listarQuery;

        public ListarHashtagQtdUseCase(IListarHashtagQtdQuery listarQuery)
        {
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<HashtagQtdOutput>> Execute()
        {
            return await _listarQuery.Execute();
        }
    }
}