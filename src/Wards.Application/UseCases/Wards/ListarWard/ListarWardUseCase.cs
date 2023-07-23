using AutoMapper;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Wards.ListarWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;
using static Wards.Utils.Fixtures.Convert;

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
                if (item.ImagemPrincipalBlob is not null && item.ImagemPrincipalBlob.Length > 0)
                {
                    item.ImagemPrincipalBase64 = ConverterBytesParaBase64(item.ImagemPrincipalBlob);
                    item.ImagemPrincipalBlob = null;
                }
            }

            return output;
        }
    }
}