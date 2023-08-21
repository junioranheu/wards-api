using Wards.Application.UseCases.Wards.Shared.Input;
using static Wards.Utils.Fixtures.Get;

namespace Wards.UnitTests.Fixtures.Mocks
{
    public static class WardMock
    {
        public static WardInput CriarInput(string titulo, string conteudo, int? usuarioId)
        {
            WardInput ward = new()
            {
                Titulo = !string.IsNullOrEmpty(titulo) ? titulo : GerarStringAleatoria(5, false),
                Conteudo = !string.IsNullOrEmpty(conteudo) ? conteudo : GerarStringAleatoria(5, false),
                ListaHashtags = new int[1],
                UsuarioId = usuarioId > 0 ? usuarioId : GerarNumeroAleatorio(1, 999),
                Usuarios = new(),
                IsAtivo = true
            };

            return ward;
        }

        public static List<WardInput> CriarListaInput()
        {
            List<WardInput> lista = new()
            {
                CriarInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), 1),
                CriarInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), 1)
            };

            return lista;
        }
    }
}