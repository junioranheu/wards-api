using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Application.UsesCases.Shared.Models;
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

        public async Task<IEnumerable<Usuario>?> Execute(PaginacaoInput input)
        {
            try
            {
                var linq = await _context.Usuarios.
                                 Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                                 Skip((input.IsSelectAll ? 0 : input.Pagina * input.Limit)).
                                 Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
                                 AsNoTracking().ToListAsync();

                return linq;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{detalhes}", DetalhesException(ex.Source));
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