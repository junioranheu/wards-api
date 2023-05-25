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

namespace Wards.UnitTests.Tests.Cidades
{
    public sealed class CidadeUseCaseTest
    {
        private readonly IMapper _map;

        public CidadeUseCaseTest()
        {
            _map = Fixture.CriarMapper();
        }

        [Theory]
        [InlineData("xxx", "xxx", true, true)]
        [InlineData("xxx", "xxx", true, true)]
        [InlineData("xxx", "xxx", true, true)]
        [InlineData("xxx", "xxx", true, true)]
        [InlineData("xxx", "xxx", true, true)]
        [InlineData("xxx", "xxx", true, true)]
        public async Task Criar_ChecarResultadoEsperado(string nome, string estadoId, bool isAtivo, bool esperado)
        {
            // Arrange;
            var criarCommand = new Mock<ICriarWardCommand>();
            criarWardCommand.Setup(x => x.Execute(It.IsAny<Ward>())).Returns(Task.FromResult(1));

            var useCase = new CriarWardUseCase(_map, criarWardCommand.Object);

            WardInput input = WardMock.CriarWardInput(titulo, conteudo, usuarioId);

            // Act;
            var resp = await useCase.Execute(input);

            // Assert;
            Assert.Equal(resp > 0, esperado);
        }
    }
}