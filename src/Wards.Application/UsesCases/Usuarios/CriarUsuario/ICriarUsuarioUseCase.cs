using Wards.Application.UsesCases.Usuarios.Shared.Input;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public interface ICriarUsuarioUseCase
    {
        Task<int> Execute(UsuarioInput input);
    }
}