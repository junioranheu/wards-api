using AutoMapper;
using Wards.Application.UseCases.Usuarios.ObterUsuario.Queries;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.ObterUsuario
{
    public sealed class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        private readonly IMapper _map;
        private readonly IObterUsuarioQuery _obterQuery;

        public ObterUsuarioUseCase(IMapper map, IObterUsuarioQuery obterQuery)
        {
            _map = map;
            _obterQuery = obterQuery;
        }

        public async Task<UsuarioOutput?> Execute(int id = 0, string email = "")
        {
            return _map.Map<UsuarioOutput>(await _obterQuery.Execute(id, email));
        }
    }
}