using Moq;
using Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData;
using Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands;
using Xunit;

namespace Wards.UnitTests.Tests.FeriadosDatas
{
    public sealed class FeriadoDataUseCaseTest
    {
        public FeriadoDataUseCaseTest()
        {

        }

        [Theory]
        [InlineData(new string[] { }, 1, false)]
        [InlineData(new string[] { "" }, 1, false)]
        [InlineData(new string[] { "01/01/0001" }, 1, false)]
        [InlineData(new string[] { "01/01/1997" }, 0, false)]
        [InlineData(new string[] { "06/02/1970", "17/01/1975", "25/03/1997", "19/12/1997" }, 1, true)]
        public async Task Criar_ChecarResultadoEsperado(string[] data, int? feriadoId, bool esperado)
        {
            // Arrange;
            var criarCommand = new Mock<ICriarFeriadoDataCommand>();
            criarCommand.Setup(x => x.Execute(It.IsAny<string[]>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var useCase = new CriarFeriadoDataUseCase(criarCommand.Object);

            // Act;
            try
            {
                await useCase.Execute(data, feriadoId.GetValueOrDefault()); // Como pegar um int?;

                // Assert;
                Assert.True(esperado);
            }
            catch (Exception)
            {
                // Assert;
                Assert.False(esperado);
            }
        }
    }
}