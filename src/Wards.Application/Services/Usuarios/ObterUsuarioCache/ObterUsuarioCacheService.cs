using Microsoft.Extensions.Caching.Memory;
using Wards.Application.UseCases.Usuarios.ObterUsuario;
using Wards.Application.UseCases.Usuarios.Shared.Output;

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

        public async Task<UsuarioOutput?> Execute(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            const string keyCache = "keyObterUsuarioCache";
            if (!_memoryCache.TryGetValue(keyCache, out UsuarioOutput? usuario))
            {
                usuario = await _obterUsuarioUseCase.Execute(email: email);
                _memoryCache.Set(keyCache, usuario, TimeSpan.FromMinutes(1));
            }

            return usuario;
        }
    }
}
