using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Wards.UnitTests.Tests.Exemplos
{
    public sealed class ExemploControllerTest
    {
        public ExemploControllerTest()
        {

        }

        [Fact]
        public async Task Testar_ExemploCancellationToken_Cancelled()
        {
            // Arrange;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act;     
            cancellationTokenSource.Cancel();  // Forçar trigger em cancellation token;
            var result = await ExemploCancellationToken(cancellationToken);

            // Assert;
            Assert.Equal("Processo cancelado pelo usuário (CancellationToken)", result.Value);
        }

        [Fact]
        public async Task Testar_ExemploCancellationToken_NotCancelled()
        {
            // Arrange;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act;
            var result = await ExemploCancellationToken(cancellationToken);

            // Assert;
            Assert.Equal("Processo finalizado com sucesso", result.Value);
        }

        #region gambi_Testar_ExemploCancellationToken
        /// <summary>
        /// Gambi pura: método copiado de "ExemplosController", já que não é correto ter dependência da API aqui na camada de testes;
        /// </summary>
        private static async Task<ActionResult<string>> ExemploCancellationToken(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(1000, cancellationToken); // Simulate a long-running request.
                return "Processo finalizado com sucesso";
            }
            catch (OperationCanceledException)
            {
                return "Processo cancelado pelo usuário (CancellationToken)";
            }
        }
        #endregion
    }
}