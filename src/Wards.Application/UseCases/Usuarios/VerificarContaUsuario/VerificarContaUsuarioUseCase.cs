using AutoMapper;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Application.UseCases.Usuarios.VerificarContaUsuario.Commands;

namespace Wards.Application.UseCases.Usuarios.VerificarContaUsuario
{
    public sealed class VerificarContaUsuarioUseCase : IVerificarContaUsuarioUseCase
    {
        private readonly IMapper _map;
        private readonly IVerificarContaUsuarioCommand _verificarContaUsuarioCommand;

        public VerificarContaUsuarioUseCase(IMapper map, IVerificarContaUsuarioCommand verificarContaUsuarioCommand)
        {
            _map = map;
            _verificarContaUsuarioCommand = verificarContaUsuarioCommand;
        }

        public async Task<UsuarioOutput?> Execute(string codigoVerificacao)
        {
            string resp = await _verificarContaUsuarioCommand.Execute(codigoVerificacao);

            if (!string.IsNullOrEmpty(resp))
            {
                return (new UsuarioOutput() { Messages = new string[] { resp } });
            }

            return new UsuarioOutput();
        }
    }
}