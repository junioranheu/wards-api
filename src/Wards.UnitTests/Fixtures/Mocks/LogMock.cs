using Wards.Application.UseCases.Logs.Shared.Input;
using static Wards.Utils.Fixtures.Get;

namespace Wards.UnitTests.Fixtures.Mocks
{
    public static class LogMock
    {
        public static LogInput CriarInput(string tipoRequisicao, string endpoint, string parametros, string descricao, int statusResposta, int usuarioId)
        {
            LogInput log = new()
            {
                TipoRequisicao = !string.IsNullOrEmpty(tipoRequisicao) ? tipoRequisicao : GerarStringAleatoria(5, false),
                Endpoint = !string.IsNullOrEmpty(endpoint) ? endpoint : GerarStringAleatoria(5, false),
                Parametros = !string.IsNullOrEmpty(parametros) ? parametros : GerarStringAleatoria(5, false),
                Descricao = !string.IsNullOrEmpty(descricao) ? descricao : GerarStringAleatoria(5, false),
                StatusResposta = statusResposta > 0 ? statusResposta : GerarNumeroAleatorio(1, 999),
                UsuarioId = usuarioId > 0 ? usuarioId : GerarNumeroAleatorio(1, 999)
            };

            return log;
        }

        public static List<LogInput> CriarListaInput()
        {
            List<LogInput> lista = new()
            {
                CriarInput(string.Empty, string.Empty, string.Empty, string.Empty, 0, 0),
                CriarInput(string.Empty, string.Empty, string.Empty, string.Empty, 0, 0)
            };

            return lista;
        }
    }
}