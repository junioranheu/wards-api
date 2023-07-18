using AutoMapper;
using Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro.Queries;
using Wards.Application.UseCases.NewsLettersCadastros.Shared.Output;
using Wards.Application.UseCases.Shared.Models.Input;

namespace Wards.Application.UseCases.NewsLettersCadastros.ListarNewsLetterCadastro
{
    public sealed class ListarNewsLetterCadastroUseCase : IListarNewsLetterCadastroUseCase
    {
        private readonly IMapper _map;
        private readonly IListarNewsLetterCadastroQuery _listarQuery;

        public ListarNewsLetterCadastroUseCase(IMapper map, IListarNewsLetterCadastroQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<NewsLetterCadastroOutput>> Execute(PaginacaoInput input)
        {
            return _map.Map<IEnumerable<NewsLetterCadastroOutput>>(await _listarQuery.Execute(input));
        }
    }
}