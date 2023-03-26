using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.VerificarContaUsuario
{
    public interface IVerificarContaUsuarioUseCase
    {
        Task<UsuarioOutput?> Execute(string codigoVerificacao);
    }
}