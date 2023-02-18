using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Autenticar.Logar
{
    public interface ILogarUseCase
    {
        Task<UsuarioDTO> Login(UsuarioDTO dto);
    }
}