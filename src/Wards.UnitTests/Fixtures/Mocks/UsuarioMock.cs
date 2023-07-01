using Wards.Application.UseCases.Usuarios.Shared.Input;
using static Wards.Utils.Fixtures.Get;

namespace Wards.UnitTests.Fixtures.Mocks
{
    public static class UsuarioMock
    {
        public static CriarUsuarioInput CriarInput(string nomeCompleto, string nomeUsuarioSistema, string email, string senha, string chamado)
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

        public static List<CriarUsuarioInput> CriarListaInput()
        {
            List<CriarUsuarioInput> lista = new()
            {
                CriarInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false)),
                CriarInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), GerarStringAleatoria(5, false))
            };

            return lista;
        }
    }
}