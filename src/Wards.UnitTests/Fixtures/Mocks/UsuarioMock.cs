using AutoMapper;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Fixtures.Get;

namespace Wards.UnitTests.Fixtures.Mocks
{
    public static class UsuarioMock
    {
        public static Usuario CriarUsuario(string nomeCompleto, string nomeUsuarioSistema, string email, string senha, string chamado)
        {
            Usuario usuario = new()
            {
                NomeCompleto = !string.IsNullOrEmpty(nomeCompleto) ? nomeCompleto : GerarStringAleatoria(5, false),
                NomeUsuarioSistema = !string.IsNullOrEmpty(nomeUsuarioSistema) ? nomeUsuarioSistema : GerarStringAleatoria(5, false),
                Email = !string.IsNullOrEmpty(email) ? email : GerarStringAleatoria(5, false),
                Senha = !string.IsNullOrEmpty(senha) ? senha : GerarStringAleatoria(5, false),
                Chamado = !string.IsNullOrEmpty(chamado) ? chamado : GerarStringAleatoria(5, false)
            };

            return usuario;
        }

        public static IEnumerable<Usuario> CriarListaUsuario()
        {
            List<Usuario> lista = new()
            {
                CriarUsuario(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false)),
                CriarUsuario(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false))
            };

            return lista;
        }

        public static CriarUsuarioInput CriarUsuarioInput(string nomeCompleto, string nomeUsuarioSistema, string email, string senha, string chamado)
        {
            CriarUsuarioInput usuario = new()
            {
                NomeCompleto = !string.IsNullOrEmpty(nomeCompleto) ? nomeCompleto : GerarStringAleatoria(5, false),
                NomeUsuarioSistema = !string.IsNullOrEmpty(nomeUsuarioSistema) ? nomeUsuarioSistema : GerarStringAleatoria(5, false),
                Email = !string.IsNullOrEmpty(email) ? email : GerarStringAleatoria(5, false),
                Senha = !string.IsNullOrEmpty(senha) ? senha : GerarStringAleatoria(5, false),
                Chamado = !string.IsNullOrEmpty(chamado) ? chamado : GerarStringAleatoria(5, false)
            };

            return usuario;
        }

        public static List<CriarUsuarioInput> CriarListaUsuarioInput()
        {
            List<CriarUsuarioInput> lista = new()
            {
                CriarUsuarioInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false)),
                CriarUsuarioInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false))
            };

            return lista;
        }

        public static async Task CriarUsuarioBanco(WardsContext context, IMapper map)
        {
            var usuarioMock = CriarUsuarioInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false));

            await context.Usuarios.AddAsync(map.Map<Usuario>(usuarioMock));
            await context.SaveChangesAsync();
        }
    }
}