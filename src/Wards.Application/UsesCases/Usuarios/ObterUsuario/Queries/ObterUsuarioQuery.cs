using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public sealed class ObterUsuarioQuery : IObterUsuarioQuery
    {
        public readonly WardsContext _context;
        private readonly IDbConnection _dbConnection;

        public ObterUsuarioQuery(WardsContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        public async Task<Usuario> Obter(int id)
        {
            string sql = "";

            return await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
        }

        public async Task<Usuario> ObterByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema)
        {
            var byEmail = await _context.Usuarios.
                          Where(e => e.Email == email).AsNoTracking().FirstOrDefaultAsync();

            if (byEmail is null)
            {
                var byNomeUsuario = await _context.Usuarios.
                                    Where(n => n.NomeUsuarioSistema == nomeUsuarioSistema).AsNoTracking().FirstOrDefaultAsync();

                if (byNomeUsuario is null)
                {
                    return null;
                }

                return byNomeUsuario;
            }

            return byEmail;
        }
    }
}
