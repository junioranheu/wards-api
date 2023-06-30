using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wards.Application.UseCases.Tokens.CriarRefreshToken;
using Wards.Application.UseCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UseCases.Usuarios.CriarUsuario;
using Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Auth.Token;
using Wards.UnitTests.Fixtures.Mocks;
using Wards.UnitTests.Fixtures;
using Xunit;
using Wards.Application.UseCases.Wards.CriarWard.Commands;
using Wards.Application.UseCases.Wards.CriarWard;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Logs.CriarLog;
using Wards.Application.UseCases.Logs.Shared.Input;

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