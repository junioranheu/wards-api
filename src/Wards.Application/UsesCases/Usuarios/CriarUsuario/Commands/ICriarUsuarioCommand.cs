using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public interface ICriarUsuarioCommand
    {
        Task<int> ExecuteAsync(UsuarioDTO dto);
    }
}