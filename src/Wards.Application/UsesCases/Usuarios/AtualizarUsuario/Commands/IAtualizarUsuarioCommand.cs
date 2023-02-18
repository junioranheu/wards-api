using Wards.Domain.DTOs;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario.Commands
{
    public interface IAtualizarUsuarioCommand
    {
        Task<int> Atualizar(UsuarioDTO dto);
    }
}