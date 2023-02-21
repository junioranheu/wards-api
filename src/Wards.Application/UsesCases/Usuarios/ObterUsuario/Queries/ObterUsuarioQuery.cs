using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public sealed class ObterUsuarioQuery : IObterUsuarioQuery
    {
        private readonly WardsContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _map;

        public ObterUsuarioQuery(
            WardsContext context,
            IDbConnection dbConnection,
            IMapper map)
        {
            _context = context;
            _dbConnection = dbConnection;
            _map = map;
        }

        public async Task<UsuarioDTO> Obter(int id)
        {
            string sql = $"SELECT * FROM Usuarios WHERE UsuarioId = {id}";
            Usuario usuario = await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });

            return _map.Map<UsuarioDTO>(usuario);
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

        public async Task<UsuarioDTO> ObterByEmail(string email)
        {
            string sql = $"SELECT * FROM Usuarios WHERE Email = '{email}'";
            Usuario usuario = await _dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { email });

            return _map.Map<UsuarioDTO>(usuario);
        }
    }
}
