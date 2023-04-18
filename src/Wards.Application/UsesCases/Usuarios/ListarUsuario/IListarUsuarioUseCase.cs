using Wards.Application.UsesCases.Shared.Models;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario
{
    public interface IListarUsuarioUseCase
    {
        Task<IEnumerable<UsuarioOutput>?> Execute(PaginacaoInput input);
    }
}