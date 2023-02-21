using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole
{
    public sealed class ListarUsuarioRoleUseCase : IListarUsuarioRoleUseCase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IListarUsuarioRoleQuery _obterQuery;

        public ListarUsuarioRoleUseCase(IMemoryCache memoryCache, IListarUsuarioRoleQuery obterQuery)
        {
            _memoryCache = memoryCache;
            _obterQuery = obterQuery;
        }

        public async Task<IEnumerable<UsuarioRole>> ListarByEmail(string email)
        {
            return await _obterQuery.ListarByEmail(email);
        }

        public async Task<IEnumerable<UsuarioRole>?> ListarUsuarioRolesByEmailComCache(dynamic context)
        {
            string email = ObterUsuarioEmailSeLogado(context);

            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            const string keyCache = "keyListarUsuarioRolesByEmailComCache";
            if (!_memoryCache.TryGetValue(keyCache, out IEnumerable<UsuarioRole>? listaUsuarioRoles))
            {
                listaUsuarioRoles = await ListarByEmail(email);

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
