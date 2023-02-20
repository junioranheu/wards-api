using Wards.Domain.DTOs;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public interface IListarUsuarioUseCase
    {
        Task<IEnumerable<UsuarioDTO>> Listar();
    }
}