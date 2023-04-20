using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Usuarios.ObterUsuario.Queries
{
    public interface IObterUsuarioQuery
    {
        Task<Usuario?> Execute(int id, string email);
    }
}