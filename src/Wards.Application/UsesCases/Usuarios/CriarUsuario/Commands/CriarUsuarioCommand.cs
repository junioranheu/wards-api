using Dapper;
using System.Data;
using System.Globalization;
using Wards.Domain.Entities;
using static Dapper.SqlMapper;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public sealed class CriarUsuarioCommand : ICriarUsuarioCommand
    {
        private readonly IDbConnection _dbConnection;

        public CriarUsuarioCommand(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Usuario> Criar(Usuario input)
        {
            string sql = $@"INSERT INTO Usuarios (NomeCompleto, NomeUsuarioSistema, Email, Senha, IsAtivo, Data)
                            VALUES ('{input.NomeCompleto}', '{input.NomeUsuarioSistema}',
                                    '{input.Email}', '{input.Senha}',
                                    {input.IsAtivo}, '{input.Data.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}');";
           
            await _dbConnection.ExecuteAsync(sql, input);

            input.UsuarioId = await _dbConnection.QueryFirstOrDefaultAsync<int>("SELECT LAST_INSERT_ID();"); ;

            return input;
        }
    }
}
