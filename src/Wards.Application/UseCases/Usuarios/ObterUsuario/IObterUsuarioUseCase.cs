using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.ObterUsuario
{
    public interface IObterUsuarioUseCase
    {
        Task<UsuarioOutput?> Execute(int id = 0, string email = "");
    }
}