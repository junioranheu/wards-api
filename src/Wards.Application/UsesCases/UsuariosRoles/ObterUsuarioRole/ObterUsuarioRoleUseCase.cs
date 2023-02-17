using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public sealed class ObterUsuarioRoleUseCase : IObterUsuarioRoleUseCase
    {
        private readonly IMemoryCache _memoryCache;
        public readonly IObterUsuarioRoleQuery _obterQuery;

        public ObterUsuarioRoleUseCase(IMemoryCache memoryCache, IObterUsuarioRoleQuery obterQuery)
        {
            _memoryCache = memoryCache;
            _obterQuery = obterQuery;
        }

        public async Task<IEnumerable<UsuarioRole>> ObterById(int id)
        {
            return await _obterQuery.ObterById(id);
        }

        public async Task<IEnumerable<UsuarioRole>> ObterByEmail(string email)
        {
            return await _obterQuery.ObterByEmail(email);
        }

        public async Task<IEnumerable<UsuarioRole>> ObterCacheObterUsuarioRolesByEmail(string email)
        {
            const string keyCache = "keyCacheUsuarioRoles";
            if (!_memoryCache.TryGetValue(keyCache, out IEnumerable<UsuarioRole> listaUsuarioRoles))
            {
                listaUsuarioRoles = await ObterByEmail(email);

                _memoryCache.Set(keyCache, listaUsuarioRoles, TimeSpan.FromMinutes(5));
            }

            return listaUsuarioRoles;
        }
    }
}
