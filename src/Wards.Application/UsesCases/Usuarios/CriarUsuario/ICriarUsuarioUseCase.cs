using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public interface ICriarUsuarioUseCase
    {
        Task<int> Execute(Usuario input);
    }
}