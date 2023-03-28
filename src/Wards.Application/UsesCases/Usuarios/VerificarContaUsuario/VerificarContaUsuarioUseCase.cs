using AutoMapper;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Application.UsesCases.Usuarios.VerificarContaUsuario.Commands;

namespace Wards.Application.UsesCases.Usuarios.VerificarContaUsuario
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