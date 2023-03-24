using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public interface ICriarUsuarioCommand
    {
        Task<Usuario> Execute(Usuario input);
    }
}