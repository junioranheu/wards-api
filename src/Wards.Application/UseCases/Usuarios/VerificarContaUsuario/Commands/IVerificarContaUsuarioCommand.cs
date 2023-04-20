using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Usuarios.VerificarContaUsuario.Commands
{
    public interface IVerificarContaUsuarioCommand
    {
        Task<string> Execute(string codigoVerificacao);
    }
}