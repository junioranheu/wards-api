using Wards.Application.UsesCases.Wards.Shared.Input;
using static Wards.Utils.Common;

namespace Wards.UnitTests.Fixtures.Mocks
{
    public static class WardMock
    {
        public static WardInput CriarWardInput(string titulo, string conteudo, int? usuarioId)
        {
            WardInput ward = new()
            {
                Titulo = !string.IsNullOrEmpty(titulo) ? titulo : Guid.NewGuid().ToString(),
                Conteudo = !string.IsNullOrEmpty(conteudo) ? conteudo : Guid.NewGuid().ToString(),
                UsuarioId = usuarioId > 0 ? usuarioId : GerarNumeroAleatorio(int.MinValue, int.MaxValue)
            };

            return ward;
        }

        public static List<WardInput> CriarListaWardInput()
        {
            List<WardInput> lista = new()
            {
                CriarWardInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), 1),
                CriarWardInput(GerarStringAleatoria(5, false), GerarStringAleatoria(5, false), 1)
            };

            return lista;
        }
    }
}