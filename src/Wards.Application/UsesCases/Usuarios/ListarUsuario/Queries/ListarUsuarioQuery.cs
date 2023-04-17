using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries
{
    public sealed class ListarUsuarioQuery : IListarUsuarioQuery
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public ListarUsuarioQuery(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>?> Execute()
        {
            try
            {
                var linq = await _context.Usuarios.
                                 Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                                 AsNoTracking().ToListAsync();

                return linq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }

        // EXEMPLO DAPPER;
        //private readonly IDbConnection _dbConnection;

        //public ListarUsuarioQuery(IDbConnection dbConnection)
        //{
        //    _dbConnection = dbConnection;
        //}

        //public async Task<IEnumerable<Usuario>?> Execute()
        //{
        //    string sql = $@"SELECT * FROM Usuarios WHERE IsAtivo = 1;";
        //    var usuarios = await _dbConnection.QueryAsync<Usuario>(sql);

        //    return usuarios;
        //}
    }
}