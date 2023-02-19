using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
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

        public async Task<IEnumerable<UsuarioRole>> Obter(int id)
        {
            return await _obterQuery.Obter(id);
        }

        public async Task<IEnumerable<UsuarioRole>> ObterByEmail(string email)
        {
            return await _obterQuery.ObterByEmail(email);
        }

        public async Task<IEnumerable<UsuarioRole>?> ObterUsuarioRolesByEmailComCache(dynamic context)
        {
            string email = ObterUsuarioEmailSeLogado(context);

            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            const string keyCache = "keyCacheUsuarioRoles";
            if (!_memoryCache.TryGetValue(keyCache, out IEnumerable<UsuarioRole>? listaUsuarioRoles))
            {
                listaUsuarioRoles = await ObterByEmail(email);

                _memoryCache.Set(keyCache, listaUsuarioRoles, TimeSpan.FromMinutes(5));
            }

            return listaUsuarioRoles;
        }

        private static string ObterUsuarioEmailSeLogado(dynamic context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Pegar o e-mail do usuário pela Azure;
                //var claim = context.HttpContext.User.Claims.First(c => c.Type == "preferred_username");
                //return claim.Value ?? string.Empty;

                // Pegar o e-mail do usuário pela autenticação própria;
                string email = context.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                return email ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
