using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;
using Wards.Domain.Entities;
using Wards.Domain.Enums;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public sealed class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        private readonly IMemoryCache _memoryCache;
        public readonly IObterUsuarioQuery _obterQuery;

        public ObterUsuarioUseCase(IMemoryCache memoryCache, IObterUsuarioQuery obterQuery)
        {
            _memoryCache = memoryCache;
            _obterQuery = obterQuery;
        }

        public async Task<Usuario> ExecuteAsync(int id)
        {
            return await _obterQuery.ExecuteAsync(id);
        }

        public async Task<int[]> GetListaIdUsuarioPerfil(string email)
        {
            const string keyCache = "keyCacheUsuarioPerfil";
            if (!_memoryCache.TryGetValue(keyCache, out int[] idUsuarioPerfilLista))
            {
                // var usuario = await ExecuteAsync(usuarioEmail); // TODO: buscar "idUsuarioPerfil" com base no "emailUsuario" acima e aplicar um FirstOrDefault();

                List<int> l = new()
                {
                    (int)UsuarioRoleEnum.Administrador,
                    (int)UsuarioRoleEnum.Comum
                };

                idUsuarioPerfilLista = l.ToArray();
                _memoryCache.Set(keyCache, idUsuarioPerfilLista, TimeSpan.FromMinutes(5));
            }

            return idUsuarioPerfilLista;
        }
    }
}
