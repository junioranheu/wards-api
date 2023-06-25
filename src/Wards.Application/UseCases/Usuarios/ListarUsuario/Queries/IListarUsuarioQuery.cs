using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Usuarios.ListarUsuario.Queries
{
    public interface IListarUsuarioQuery
    {
        Task<IEnumerable<Usuario>> Execute(PaginacaoInput input);
    }
}