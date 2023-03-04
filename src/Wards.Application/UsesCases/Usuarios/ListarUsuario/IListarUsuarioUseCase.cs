using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public interface IListarUsuarioUseCase
    {
        Task<IEnumerable<Usuario>?> Listar();
    }
}