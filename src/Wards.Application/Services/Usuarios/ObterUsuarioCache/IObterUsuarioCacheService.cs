using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.Services.Usuarios.ObterUsuarioCache
{
    public interface IObterUsuarioCacheService
    {
        Task<UsuarioOutput?> Execute(string email);
    }
}