using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario
{
    public interface IAtualizarUsuarioUseCase
    {
        Task<int> ExecuteAsync(UsuarioDTO dto);
    }
}