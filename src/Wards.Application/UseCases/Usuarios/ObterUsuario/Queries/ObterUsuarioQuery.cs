using Microsoft.EntityFrameworkCore;
using System.Data;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UseCases.Usuarios.ObterUsuario.Queries
{
    public sealed class ObterUsuarioQuery : IObterUsuarioQuery
    {
        private readonly WardsContext _context;

        public ObterUsuarioQuery(WardsContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> Execute(int id, string email)
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

        // EXEMPLO DAPPER;
        //public async Task<UsuarioDTO> Obter(int id)
        //{
        //    string sql = $"SELECT * FROM Usuarios WHERE UsuarioId = {id}";
        //    Usuario usuario = await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });

        //    return _map.Map<UsuarioDTO>(usuario);
        //}
    }
}
