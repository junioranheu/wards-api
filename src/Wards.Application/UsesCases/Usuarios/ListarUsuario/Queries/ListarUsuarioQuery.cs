using AutoMapper;
using Dapper;
using System.Data;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries
{
    public sealed class ListarUsuarioQuery : IListarUsuarioQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _map;

        public ListarUsuarioQuery(IDbConnection dbConnection, IMapper map)
        {
            _dbConnection = dbConnection;
            _map = map;
        }

        public async Task<IEnumerable<UsuarioDTO>> Listar()
        {
            string sql = $@"SELECT * FROM Usuarios WHERE IsAtivo = 1;";
            var usuarios = await _dbConnection.QueryAsync<Usuario>(sql);

            return _map.Map<IEnumerable<UsuarioDTO>>(usuarios);
        }
    }
}
