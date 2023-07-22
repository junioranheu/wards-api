using AutoMapper;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Wards.ListarWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;

namespace Wards.Application.UseCases.Wards.ListarWard
{
    public sealed class ListarWardUseCase : IListarWardUseCase
    {
        private readonly IMapper _map;
        private readonly IListarWardQuery _listarQuery;

        public ListarWardUseCase(IMapper map, IListarWardQuery listarQuery)
        {
            _map = map;
            _listarQuery = listarQuery;
        }

        public async Task<IEnumerable<WardOutput>> Execute(PaginacaoInput input)
        {
            var output = _map.Map<IEnumerable<WardOutput>>(await _listarQuery.Execute(input));

            foreach (var item in output)
            {
                if (item.BlobImagemPrincipal is not null)
                {
                    item.FormFileImagemPrincipal = CONVERTERBLOBPARAIFORMFILE();
                    item.BlobImagemPrincipal = null;
                }
            }

            // FAZER ESSA MESMA COISA PRO OBTER TAMBÉM!!!!!!!!!!

            return output;
        }
    }
}