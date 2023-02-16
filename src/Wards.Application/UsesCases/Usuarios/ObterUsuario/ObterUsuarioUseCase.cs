using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public sealed class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        private readonly IMemoryCache _memoryCache;
        public readonly IObterUsuarioQuery _obterUsuarioQuery;

        public ObterUsuarioUseCase(IMemoryCache memoryCache, IObterUsuarioQuery obterUsuarioQuery)
        {
            _memoryCache = memoryCache;
            _obterUsuarioQuery = obterUsuarioQuery;
        }

        public async Task<Usuario> ExecuteAsync(int id)
        {
            return await _obterUsuarioQuery.ExecuteAsync(id);
        }

        public async Task<int[]> GetListaIdUsuarioPerfil(string email)
        {
            const string keyCache = "keyCacheUsuarioPerfil";
            if (!_memoryCache.TryGetValue(keyCache, out int[] idUsuarioPerfilLista))
            {
                // var usuario = await ExecuteAsync(usuarioEmail); // TODO: buscar "idUsuarioPerfil" com base no "emailUsuario" acima e aplicar um FirstOrDefault();

                List<int> l = new()
                {
                    (int)UsuarioPerfilEnum.Suporte,
                    (int)UsuarioPerfilEnum.Publico
                };

                idUsuarioPerfilLista = l.ToArray();
                _memoryCache.Set(keyCache, idUsuarioPerfilLista, TimeSpan.FromMinutes(5));
            }

            return idUsuarioPerfilLista;
        }
    }
}
