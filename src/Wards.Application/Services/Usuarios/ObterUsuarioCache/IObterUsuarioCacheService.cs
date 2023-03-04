using Wards.Domain.Entities;

namespace Wards.Application.Services.Usuarios.ObterUsuarioCache
{
    public interface IObterUsuarioCacheService
    {
        Task<Usuario?> Execute(string email);
    }
}