using Microsoft.EntityFrameworkCore;
using System.Data;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public sealed class CriarUsuarioCommand : ICriarUsuarioCommand
    {
        private readonly WardsContext _context;

        public CriarUsuarioCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<Usuario> Execute(Usuario input)
        {
            await AtualizarFlags(input);

            await _context.AddAsync(input);
            await _context.SaveChangesAsync();

            return input;
        }

        private async Task AtualizarFlags(Usuario input)
        {
            var linq = await _context.Usuarios.
                             Where(u => u.Email == input.Email && u.IsAtivo == true).
                             AsNoTracking().ToListAsync();

            if (linq.Count > 0)
            {
                foreach (var l in linq)
                {
                    l.IsAtivo = false;
                    l.IsLatest = false;
                }

                _context.UpdateRange(linq);
            }
        }

        // EXEMPLO DAPPER;
        // public async Task<Usuario> Criar(Usuario input)
        // {
        //     string sql = $@"INSERT INTO Usuarios (NomeCompleto, NomeUsuarioSistema, Email, Senha, IsAtivo, Data)
        //                     VALUES ('{input.NomeCompleto}', '{input.NomeUsuarioSistema}',
        //                             '{input.Email}', '{input.Senha}',
        //                             {input.IsAtivo}, '{input.Data.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}');";

        //     await _dbConnection.Execute(sql, input);

        //     input.UsuarioId = await _dbConnection.QueryFirstOrDefaultAsync<int>("SELECT LAST_INSERT_ID();"); ;

        //     return input;
        // }
    }
}
