using Wards.Domain.DTOs;

namespace Wards.Application.Services.Usuarios.ObterUsuarioCache
{
    public interface IObterUsuarioCacheService
    {
        Task<UsuarioDTO?> ObterUsuarioCache(string email);
    }
}