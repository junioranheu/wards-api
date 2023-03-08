using Microsoft.EntityFrameworkCore;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries
{
    public sealed class ListarUsuarioQuery : IListarUsuarioQuery
    {
        private readonly WardsContext _context;

        public ListarUsuarioQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>?> Execute()
        {
            var linq = await _context.Usuarios.
                             Include(ur => ur.UsuarioRoles).
                             AsNoTracking().ToListAsync();

            return linq;
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