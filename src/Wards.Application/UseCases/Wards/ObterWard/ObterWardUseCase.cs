using AutoMapper;
using Wards.Application.UseCases.Wards.ObterWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Output;
using static Wards.Utils.Fixtures.Convert;

namespace Wards.Application.UseCases.Wards.ObterWard
{
    public sealed class ObterWardUseCase : IObterWardUseCase
    {
        private readonly IMapper _map;
        private readonly IObterWardQuery _obterQuery;

        public ObterWardUseCase(IMapper map, IObterWardQuery obterQuery)
        {
            _map = map;
            _obterQuery = obterQuery;
        }

        public async Task<WardOutput?> Execute(int id)
        {
            var output = _map.Map<WardOutput>(await _obterQuery.Execute(id));

            if (output.ImagemPrincipalBlob is not null && output.ImagemPrincipalBlob.Length > 0)
            {
                output.ImagemPrincipalBase64 = ConverterBytesParaBase64(output.ImagemPrincipalBlob);
                output.ImagemPrincipalBlob = null;
            }

            return output;
        }
    }
}