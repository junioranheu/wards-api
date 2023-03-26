using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries
{
    public sealed class ObterUsuarioCondicaoArbitrariaQuery : IObterUsuarioCondicaoArbitrariaQuery
    {
        private readonly WardsContext _context;

        public ObterUsuarioCondicaoArbitrariaQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<(Usuario?, string)> Execute(string login)
        {
            var byEmail = await _context.Usuarios.
                          Where(e => e.Email == login).
                          Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                          AsNoTracking().FirstOrDefaultAsync();

            if (byEmail is null)
            {
                var byNomeUsuario = await _context.Usuarios.
                                    Where(n => n.NomeUsuarioSistema == login).
                                    Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                                    AsNoTracking().FirstOrDefaultAsync();

                if (byNomeUsuario is null)
                {
                    return (null, string.Empty);
                }

                return (byNomeUsuario, byNomeUsuario.Senha ?? string.Empty);
            }

            return (byEmail, byEmail.Senha ?? string.Empty);
        }
    }
}