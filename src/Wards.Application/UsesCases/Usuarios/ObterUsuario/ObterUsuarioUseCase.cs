using AutoMapper;
using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
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

        public async Task<UsuarioDTO?> Obter(int id = 0, string email = "")
        {
            Usuario? usuario = await _obterQuery.Obter(id, email);
            return _map.Map<UsuarioDTO?>(usuario);
        }

        public async Task<Usuario> ObterByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema)
        {
            return await _obterQuery.ObterByEmailOuUsuarioSistema(email, nomeUsuarioSistema);
        }
    }
}
