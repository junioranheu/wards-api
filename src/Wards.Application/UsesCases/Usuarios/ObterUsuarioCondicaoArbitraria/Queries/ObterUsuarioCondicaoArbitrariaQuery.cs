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

        public async Task<Usuario> Execute(string? email, string? nomeUsuarioSistema)
        {
            var byEmail = await _context.Usuarios.
                          Where(e => e.Email == email).
                          AsNoTracking().FirstOrDefaultAsync();

            if (byEmail is null)
            {
                var byNomeUsuario = await _context.Usuarios.
                                    Where(n => n.NomeUsuarioSistema == nomeUsuarioSistema).
                                    AsNoTracking().FirstOrDefaultAsync();

                if (byNomeUsuario is null)
                {
                    return new Usuario();
                }

                return byNomeUsuario;
            }

            return byEmail;
        }
    }
}
