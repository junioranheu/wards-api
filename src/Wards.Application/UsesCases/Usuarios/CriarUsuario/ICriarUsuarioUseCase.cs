using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public interface ICriarUsuarioUseCase
    {
        Task<int> ExecuteAsync(UsuarioDTO dto);
    }
}