using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Usuarios.ListarUsuario.Queries
{
    public sealed class ListarUsuarioQuery : IListarUsuarioQuery
    {
        private readonly WardsContext _context;

        public ListarUsuarioQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> Execute(PaginacaoInput input)
        {
            var linq = await _context.Usuarios.
                       Include(ur => ur.UsuarioRoles)!.ThenInclude(r => r.Roles).
                       OrderBy(u => u.UsuarioId).
                       Skip((input.IsSelectAll ? 0 : input.Index * input.Limit)).
                       Take((input.IsSelectAll ? int.MaxValue : input.Limit)).
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