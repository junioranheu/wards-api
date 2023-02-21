using Microsoft.EntityFrameworkCore;
using System.Data;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries
{
    public sealed class ListarUsuarioRoleQuery : IListarUsuarioRoleQuery
    {
        private readonly WardsContext _context;

        public ListarUsuarioRoleQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioRole>> ListarByEmail(string email)
        {
            var ByEmail = await _context.UsuariosRoles.
                                 Include(u => u.Usuarios).
                                 Where(u => u.Usuarios.Email == email && u.Usuarios.IsAtivo == true).
                                 AsNoTracking().ToListAsync();

            foreach (var item in ByEmail)
            {
                item.Usuarios.Senha = string.Empty;
            }

            return ByEmail;
        }
    }
}
