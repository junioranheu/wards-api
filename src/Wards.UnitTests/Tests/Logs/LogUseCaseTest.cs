using AutoMapper;
using Moq;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Domain.Entities;
using Wards.UnitTests.Fixtures;
using Wards.UnitTests.Fixtures.Mocks;
using Xunit;

namespace Wards.UnitTests.Tests.Logs
{
    public sealed class LogUseCaseTest
    {
        private readonly IMapper _map;

        public LogUseCaseTest()
        {
            _map = Fixture.CriarMapper();
        }

        [Theory]
        [InlineData("xxx", "xxx", "xxx", "xxx", 0, 0, true)]
        public async Task Criar_ChecarResultadoEsperado(string tipoRequisicao, string endpoint, string parametros, string descricao, int statusResposta, int usuarioId, bool esperado)
        {
            // Arrange;
            var criarCommand = new Mock<ICriarLogCommand>();
            criarCommand.Setup(x => x.Execute(It.IsAny<Log>())).Returns(Task.FromResult(1));

            var useCase = new CriarLogUseCase(_map, criarCommand.Object);

            LogInput input = LogMock.CriarLogInput(tipoRequisicao, endpoint, parametros, descricao, statusResposta, usuarioId);

            // Act;
            await useCase.Execute(input);

            // Assert;
            //Assert.Equal(esperado);
        }
    }
}