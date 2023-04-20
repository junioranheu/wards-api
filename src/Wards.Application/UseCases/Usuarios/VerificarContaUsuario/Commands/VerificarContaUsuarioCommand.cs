using Microsoft.EntityFrameworkCore;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Usuarios.VerificarContaUsuario.Commands
{
    public sealed class VerificarContaUsuarioCommand : IVerificarContaUsuarioCommand
    {
        private readonly WardsContext _context;

        public VerificarContaUsuarioCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<string> Execute(string codigoVerificacao)
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
    }
}