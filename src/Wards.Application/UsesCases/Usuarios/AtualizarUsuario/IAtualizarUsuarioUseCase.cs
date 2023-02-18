using Wards.Domain.DTOs;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario
{
    public interface IAtualizarUsuarioUseCase
    {
        Task<int> Atualizar(UsuarioDTO dto);
    }
}