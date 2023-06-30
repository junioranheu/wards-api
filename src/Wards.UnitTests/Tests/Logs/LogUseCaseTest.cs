using AutoMapper;
using Moq;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Logs.ListarLog;
using Wards.Application.UseCases.Logs.ListarLog.Queries;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Application.UseCases.Shared.Models.Input;
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
        [InlineData("POST", "TesteController/Teste", "Teste", "Isso é um teste válido", 200, 1, true)]
        [InlineData("", "", "", "Isso é um teste inválido", 0, 0, false)]
        public async Task Criar_ChecarResultadoEsperado(string tipoRequisicao, string endpoint, string parametros, string descricao, int statusResposta, int usuarioId, bool esperado)
        {
            // Arrange;
            var criarCommand = new Mock<ICriarLogCommand>();
            criarCommand.Setup(x => x.Execute(It.IsAny<Log>())).Returns(Task.FromResult(1));

            var useCase = new CriarLogUseCase(_map, criarCommand.Object);

            LogInput input = LogMock.CriarLogInput(tipoRequisicao, endpoint, parametros, descricao, statusResposta, usuarioId);

            try
            {
                // Act;
                await useCase.Execute(input);

                // Assert;
                Assert.True(esperado);
            }
            catch (Exception)
            {
                // Assert;
                Assert.False(esperado);
            }
        }

        [Fact]
        public async Task Listar_ChecarResultadoEsperado()
        {
            // Arrange;
            var paginacao = new Mock<PaginacaoInput>();
            var listarQuery = new Mock<IListarLogQuery>();
            listarQuery.Setup(x => x.Execute(It.IsAny<PaginacaoInput>())).Returns(Task.FromResult(LogMock.CriarListaLog()));

            var useCase = new ListarLogUseCase(_map, listarQuery.Object);

            // Act;
            var teste = await useCase.Execute(paginacao.Object);

            // Assert;
            Assert.True(teste.Count() > 0);
        }
    }
}