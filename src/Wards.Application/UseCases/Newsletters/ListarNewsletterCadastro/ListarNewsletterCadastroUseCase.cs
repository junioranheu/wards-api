using AutoMapper;
using Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro.Queries;
using Wards.Application.UseCases.NewslettersCadastros.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.NewslettersCadastros.ListarNewsletterCadastro
{
    public sealed class ListarNewsletterCadastroUseCase : IListarNewsletterCadastroUseCase
    {
        private readonly IMapper _map;
        private readonly IListarNewsletterCadastroQuery _listarQuery;

        public ListarNewsletterCadastroUseCase(IMapper map, IListarNewsletterCadastroQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<NewsletterCadastroOutput>> Execute(PaginacaoInput input)
        {
            return _map.Map<IEnumerable<NewsletterCadastroOutput>>(await _listarQuery.Execute(input));
        }
    }
}