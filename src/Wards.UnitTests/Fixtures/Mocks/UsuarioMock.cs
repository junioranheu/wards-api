using AutoMapper;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.UnitTests.Fixtures.Mocks
{
    public static class UsuarioMock
    {
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

        public static async Task CriarUsuarioBanco(WardsContext context, IMapper map)
        {
            var usuarioMock = CriarUsuarioInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false));
            await context.Usuarios.AddAsync(map.Map<Usuario>(usuarioMock));
            await context.SaveChangesAsync();
        }
    }
}