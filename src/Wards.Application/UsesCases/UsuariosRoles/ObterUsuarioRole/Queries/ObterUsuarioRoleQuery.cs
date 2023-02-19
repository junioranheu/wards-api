using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries
{
    public sealed class ObterUsuarioRoleQuery : IObterUsuarioRoleQuery
    {
        private readonly WardsContext _context;

        public ObterUsuarioRoleQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioRole>> ObterByUsuarioEmail(string email)
        {
            var byUsuarioEmail = await _context.UsuariosRoles.
                                 Include(u => u.Usuarios).
                                 Where(u => u.Usuarios.Email == email && u.Usuarios.IsAtivo == true).
                                 AsNoTracking().ToListAsync();

            foreach (var item in byUsuarioEmail)
            {
                item.Usuarios.Senha = string.Empty;
            }

            return byUsuarioEmail;
        }
    }
}
