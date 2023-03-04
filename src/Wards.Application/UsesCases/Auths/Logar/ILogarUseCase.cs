using Wards.Application.UsesCases.Usuarios.Shared.Input;

namespace Wards.Application.UsesCases.Auths.Logar
{
    public interface ILogarUseCase
    {
        Task<(UsuarioInput?, string)> Execute(UsuarioInput input);
    }
}