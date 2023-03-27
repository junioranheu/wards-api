using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Wards.Domain.Entities;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public sealed class CriarUsuarioCommand : ICriarUsuarioCommand
    {
        private readonly WardsContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CriarUsuarioCommand(WardsContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Usuario> Execute(Usuario input)
        {
            await AtualizarFlags(input);

            await _context.AddAsync(input);
            await _context.SaveChangesAsync();

            input.Foto = GerarNomeFoto(input);

            if (!string.IsNullOrEmpty(input.Foto))
            {
                string caminhoNovaImagem = await UparImagem(file, input.Foto, GetDescricaoEnum(CaminhoUploadEnum.FotoPerfilUsuario), string.Empty, _webHostEnvironment);
            }

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

        private static string GerarNomeFoto(Usuario input)
        {
            return $"{input.UsuarioId}{GerarStringAleatoria(5, true)}.jpg";
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