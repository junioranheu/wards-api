using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.AutenticarUsuario
{
    public interface IAutenticarUsuarioUseCase
    {
        Task<AutenticarUsuarioOutput> Execute(AutenticarUsuarioInput input);
    }
}