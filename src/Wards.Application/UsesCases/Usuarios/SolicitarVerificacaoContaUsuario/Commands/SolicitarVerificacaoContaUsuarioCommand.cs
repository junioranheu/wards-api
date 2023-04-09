using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.SolicitarVerificacaoContaUsuario.Commands
{
    public sealed class SolicitarVerificacaoContaUsuarioCommand : ISolicitarVerificacaoContaUsuarioCommand
    {
        private readonly WardsContext _context;

        public SolicitarVerificacaoContaUsuarioCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<(string mensagemErro, string email, string nomeCompleto, string codigoVerificacao)> Execute(int usuarioId)
        {
            var linq = await _context.Usuarios.FindAsync(usuarioId);

            if (linq is null)
            {
                return (ObterDescricaoEnum(CodigoErroEnum.NaoEncontrado), string.Empty, string.Empty, string.Empty);
            }

            // #1 - Verificar se conta já estava ativa;
            if (linq.IsVerificado)
            {
                return (ObterDescricaoEnum(CodigoErroEnum.ContaJaVerificada), string.Empty, string.Empty, string.Empty);
            }

            // #2 - Se o código estiver válido, não envie outro e-mail;
            if (linq?.ValidadeCodigoVerificacao >= HorarioBrasilia())
            {
                return (ObterDescricaoEnum(CodigoErroEnum.EmailValidacaoJaEnviado), string.Empty, string.Empty, string.Empty);
            }

            // #3 - Gerar código de verificação e atualizar;
            string codigoVerificacao = GerarStringAleatoria(6, true);
            linq!.CodigoVerificacao = codigoVerificacao;
            linq.ValidadeCodigoVerificacao = HorarioBrasilia().AddHours(24);

            _context.Update(linq);
            await _context.SaveChangesAsync();

            return (string.Empty, linq.Email ?? string.Empty, linq.NomeCompleto ?? string.Empty, linq.CodigoVerificacao ?? string.Empty);
        }
    }
}