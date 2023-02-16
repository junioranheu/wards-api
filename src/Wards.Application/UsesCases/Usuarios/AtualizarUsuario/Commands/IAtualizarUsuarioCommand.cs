using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario.Commands
{
    public interface IAtualizarUsuarioCommand
    {
        Task<int> ExecuteAsync(UsuarioDTO dto);
    }
}