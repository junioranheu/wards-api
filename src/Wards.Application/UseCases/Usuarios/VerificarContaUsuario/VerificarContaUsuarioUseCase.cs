using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Application.UseCases.Usuarios.VerificarContaUsuario.Commands;

namespace Wards.Application.UseCases.Usuarios.VerificarContaUsuario
{
    public sealed class VerificarContaUsuarioUseCase : IVerificarContaUsuarioUseCase
    {
        private readonly IVerificarContaUsuarioCommand _verificarContaUsuarioCommand;

        public VerificarContaUsuarioUseCase(IVerificarContaUsuarioCommand verificarContaUsuarioCommand)
        {
            _verificarContaUsuarioCommand = verificarContaUsuarioCommand;
        }

        public async Task<UsuarioOutput?> Execute(string codigoVerificacao)
        {
            string resp = await _verificarContaUsuarioCommand.Execute(codigoVerificacao);

            if (!string.IsNullOrEmpty(resp))
            {
                throw new Exception(resp);
            }

            return new UsuarioOutput();
        }
    }
}