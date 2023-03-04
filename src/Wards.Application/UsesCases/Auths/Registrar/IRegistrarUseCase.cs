using Wards.Application.UsesCases.Usuarios.Shared.Input;

namespace Wards.Application.UsesCases.Auths.Registrar
{
    public interface IRegistrarUseCase
    {
        Task<(UsuarioInput?, string)> Execute(UsuarioInput input);
    }
}