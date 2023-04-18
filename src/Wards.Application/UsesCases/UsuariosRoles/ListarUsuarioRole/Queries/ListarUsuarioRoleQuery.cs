using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.UsuariosRoles.ObterUsuarioRole.Queries
{
    public sealed class ListarUsuarioRoleQuery : IListarUsuarioRoleQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ListarUsuarioRoleQuery(WardsContext context, ILogger<ListarUsuarioRoleQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UsuarioRole>> Execute(string email)
        {
            try
            {
                var linq = await _context.UsuariosRoles.
                                 Include(u => u.Usuarios).
                                 Where(u => u.Usuarios!.Email == email && u.Usuarios.IsAtivo == true).
                                 AsNoTracking().ToListAsync();

                return linq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }
    }
}