using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public sealed class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IObterUsuarioQuery _obterQuery;

        public ObterUsuarioUseCase(IMemoryCache memoryCache, IObterUsuarioQuery obterQuery)
        {
            _memoryCache = memoryCache;
            _obterQuery = obterQuery;
        }

        public async Task<UsuarioDTO> Obter(int id)
        {
            return await _obterQuery.Obter(id);
        }

        public async Task<Usuario> ObterByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema)
        {
            return await _obterQuery.ObterByEmailOuUsuarioSistema(email, nomeUsuarioSistema);
        }

        public async Task<UsuarioDTO> ObterByEmail(string email)
        {
            return await _obterQuery.ObterByEmail(email);
        }

        public async Task<UsuarioDTO?> ObterByEmailComCache(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            const string keyCache = "keyObterByEmailComCache";
            if (!_memoryCache.TryGetValue(keyCache, out UsuarioDTO? dto))
            {
                dto = await ObterByEmail(email);

                _memoryCache.Set(keyCache, dto, TimeSpan.FromMinutes(5));
            }

            return dto;
        }
    }
}
