using Wards.Application.UseCases.Hashtags.ListarHashtag.Queries;

namespace Wards.Application.UseCases.Hashtags.ListarHashtag
{
    public sealed class ListarHashtagUseCase : IListarHashtagUseCase
    {
        private readonly IListarHashtagQuery _listarQuery;

        public ListarHashtagUseCase(IListarHashtagQuery listarQuery)
        {
            _listarQuery = listarQuery;
        }

        public async Task<List<string>> Execute()
        {
            var linq = await _listarQuery.Execute();
            List<string> output = new();

            foreach (var item in linq)
            {
                output.Add(item.Tag);
            }

            return output;
        }
    }
}