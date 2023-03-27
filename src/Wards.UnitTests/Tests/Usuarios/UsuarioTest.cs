using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Auth.Token;
using Xunit;

namespace Wards.UnitTests.Tests.Usuarios
{
    public sealed class UsuarioTest
    {
        [Fact]
        public async Task CriarUsuarioUseCase_DeveRetornar1_QuandoParametrosValidosEBanco()
        {
            // Arrange;
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            var mapper = new Mock<IMapper>();
            var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            var criarUsuarioCommand = new Mock<ICriarUsuarioCommand>();
            var criarUsuarioCondicaoArbitrariaUseCase = new Mock<IObterUsuarioCondicaoArbitrariaUseCase>();
            var criarRefreshTokenUseCase = new Mock<ICriarRefreshTokenUseCase>();

            Usuario u = new() { NomeCompleto = "Junior" };
            criarUsuarioCommand.Setup(x => x.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(u));
     
            var sut = new CriarUsuarioUseCase(webHostEnvironment.Object, mapper.Object, jwtTokenGenerator.Object, criarUsuarioCommand.Object, criarUsuarioCondicaoArbitrariaUseCase.Object, criarRefreshTokenUseCase.Object);
            var input = new CriarUsuarioInput() { Email = "x@gmail.com" };

            // Act;
            var resp = await sut.Execute(input);

            // Assert;
            Assert.True(resp.UsuarioId > 0);
        }

        [Fact]
        public async Task CriarUsuarioUseCase_DeveRetornar0_QuandoFalhaNoBanco()
        {
            //// Arrange;
            //var map = new Mock<IMapper>();
            //var command = new Mock<ICriarUsuarioCommand>();
            //Usuario u = new() { UsuarioId = 0 };
            //command.Setup(x => x.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(u));

            //var sut = new CriarUsuarioUseCase(map.Object, command.Object);
            //var input = new CriarUsuarioInput() { Email = string.Empty };

            //// Act;
            //var resp = await sut.Execute(input);

            //// Assert;
            //Assert.True(resp.UsuarioId < 1);
        }
    }
}