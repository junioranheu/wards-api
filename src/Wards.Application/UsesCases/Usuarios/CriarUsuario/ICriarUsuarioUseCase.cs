using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public interface ICriarUsuarioUseCase
    {
        Task<UsuarioOutput?> Execute(CriarUsuarioInput input);
    }
}