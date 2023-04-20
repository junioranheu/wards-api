using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Usuarios.CriarUsuario.Commands
{
    public interface ICriarUsuarioCommand
    {
        Task<Usuario> Execute(Usuario input);
    }
}