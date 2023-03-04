using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole;
using Wards.Domain.Entities;

namespace Wards.Application.Services.Usuarios.ListarUsuarioRolesCache
{
    public sealed class ListarUsuarioRolesCacheService : IListarUsuarioRolesCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IListarUsuarioRoleUseCase _listarUsuarioRolelUseCase;

        public ListarUsuarioRolesCacheService(
            IMemoryCache memoryCache,
            IListarUsuarioRoleUseCase listarUsuarioPerfilUseCase)
        {
            _memoryCache = memoryCache;
            _listarUsuarioRolelUseCase = listarUsuarioPerfilUseCase;
        }

        public async Task<IEnumerable<UsuarioRole>?> Execute(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            const string keyCache = "keyListarUsuarioRolesCache";
            if (!_memoryCache.TryGetValue(keyCache, out IEnumerable<UsuarioRole>? listaUsuarioRoles))
            {
                listaUsuarioRoles = await _listarUsuarioRolelUseCase.Execute(email);
                _memoryCache.Set(keyCache, listaUsuarioRoles, TimeSpan.FromMinutes(1));
            }

            return listaUsuarioRoles;
        }
    }
}
