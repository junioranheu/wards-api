﻿using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UseCases.UsuariosRoles.ObterUsuarioRole;
using Wards.Application.UseCases.UsuariosRoles.Shared.Output;
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

        public async Task<IEnumerable<UsuarioRoleOutput>?> Execute(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            string keyCache = $"keyListarUsuarioRolesCache_{email}";
            if (!_memoryCache.TryGetValue(keyCache, out IEnumerable<UsuarioRoleOutput>? listaUsuarioRoles))
            {
                listaUsuarioRoles = await _listarUsuarioRolelUseCase.Execute(email);
                _memoryCache.Set(keyCache, listaUsuarioRoles, TimeSpan.FromMinutes(1));
            }

            return listaUsuarioRoles;
        }
    }
}
