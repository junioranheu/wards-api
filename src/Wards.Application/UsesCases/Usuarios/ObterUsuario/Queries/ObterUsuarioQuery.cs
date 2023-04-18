using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public sealed class ObterUsuarioQuery : IObterUsuarioQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ObterUsuarioQuery(WardsContext context, ILogger<ObterUsuarioQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Usuario?> Execute(int id, string email)
        {
            try
            {
                var linq = await _context.Usuarios.
                                 Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                                 Where(u =>
                                     id > 0 ? u.UsuarioId == id : true
                                     && !string.IsNullOrEmpty(email) ? u.Email == email : true
                                     && u.IsLatest == true // É necessário ser o último para referenciar o "UsuarioPerfis" atual; 
                                 ).AsNoTracking().FirstOrDefaultAsync();

                return linq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
                throw;
            }
        }

        // EXEMPLO DAPPER;
        //public async Task<UsuarioDTO> Obter(int id)
        //{
        //    string sql = $"SELECT * FROM Usuarios WHERE UsuarioId = {id}";
        //    Usuario usuario = await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });

        //    return _map.Map<UsuarioDTO>(usuario);
        //}
    }
}
