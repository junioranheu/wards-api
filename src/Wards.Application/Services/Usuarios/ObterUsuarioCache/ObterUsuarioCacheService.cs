using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Domain.Entities;

namespace Wards.Application.Services.Usuarios.ObterUsuarioCache
{
    public sealed class ObterUsuarioCacheService : IObterUsuarioCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IObterUsuarioUseCase _obterUsuarioUseCase;

        public ObterUsuarioCacheService(
            IMemoryCache memoryCache,
            IObterUsuarioUseCase obterUsuarioUseCase)
        {
            _memoryCache = memoryCache;
            _obterUsuarioUseCase = obterUsuarioUseCase;
        }

        public async Task<Usuario?> Execute(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            const string keyCache = "keyObterUsuarioCache";
            if (!_memoryCache.TryGetValue(keyCache, out Usuario? usuario))
            {
                usuario = await _obterUsuarioUseCase.Execute(email: email);
                _memoryCache.Set(keyCache, usuario, TimeSpan.FromMinutes(1));
            }

            return usuario;
        }
    }
}