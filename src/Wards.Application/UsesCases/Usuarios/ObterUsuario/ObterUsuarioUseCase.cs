using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public sealed class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        private readonly IObterUsuarioQuery _obterQuery;

        public ObterUsuarioUseCase(IObterUsuarioQuery obterQuery)
        {
            _obterQuery = obterQuery;
        }

        public async Task<Usuario?> Execute(int id = 0, string email = "")
        {
            return await _obterQuery.Execute(id, email);
        }
    }
}