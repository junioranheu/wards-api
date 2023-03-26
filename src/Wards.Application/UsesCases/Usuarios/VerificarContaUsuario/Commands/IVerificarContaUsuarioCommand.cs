using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.VerificarContaUsuario.Commands
{
    public interface IVerificarContaUsuarioCommand
    {
        Task<string> Execute(string codigoVerificacao);
    }
}