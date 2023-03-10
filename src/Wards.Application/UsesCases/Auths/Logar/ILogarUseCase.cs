using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Auths.Logar
{
    public interface ILogarUseCase
    {
        Task<(UsuarioOutput?, string)> Execute(UsuarioInput input);
    }
}