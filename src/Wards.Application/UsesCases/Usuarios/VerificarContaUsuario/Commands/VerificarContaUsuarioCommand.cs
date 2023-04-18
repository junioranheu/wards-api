using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.VerificarContaUsuario.Commands
{
    public sealed class VerificarContaUsuarioCommand : IVerificarContaUsuarioCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public VerificarContaUsuarioCommand(WardsContext context, ILogger<VerificarContaUsuarioCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> Execute(string codigoVerificacao)
        {
            try
            {
                var linq = await _context.Usuarios.
                 Where(u => u.CodigoVerificacao == codigoVerificacao).
                 AsNoTracking().FirstOrDefaultAsync();

                if (linq is null)
                {
                    return ObterDescricaoEnum(CodigoErroEnum.CodigoVerificacaoInvalido);
                }

                if (HorarioBrasilia() > linq.ValidadeCodigoVerificacao)
                {
                    return ObterDescricaoEnum(CodigoErroEnum.CodigoExpirado);
                }

                if (linq.IsVerificado)
                {
                    return ObterDescricaoEnum(CodigoErroEnum.ContaJaVerificada);
                }

                linq.IsVerificado = true;
                _context.Update(linq);
                await _context.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}