using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public sealed class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        public readonly IObterUsuarioQuery _obterQuery;

        public ObterUsuarioUseCase(IObterUsuarioQuery obterQuery)
        {
            _obterQuery = obterQuery;
        }

        public async Task<Usuario> ExecuteAsync(int id)
        {
            return await _obterQuery.ExecuteAsync(id);
        }
    }
}
