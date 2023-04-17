using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Wards.Application.AutoMapper;
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
        private readonly IMapper _map;

        public UsuarioTest()
        {
            var mockMapper = new MapperConfiguration(x =>
            {
                x.AddProfile(new AutoMapperConfig());
            });

             _map = mockMapper.CreateMapper();
        }

        [Theory]
        [InlineData("Junior de Souza", "junioranheu", "junioranheu@gmail.com", "Juninho26@", "#1")]
        //[InlineData("Otávio Villas Boas", "otavioGOD", "otavio@gmail.com", "Otavinho26@", "#2")]
        //[InlineData("Mariana Scalzaretto", "elfamscal", "elfa@gmail.com", "Marianinha26@", "#3")]
        public async Task CriarUsuarioUseCase_DeveRetornar1_QuandoParametrosValidosEBanco(string nomeCompleto, string nomeUsuarioSistema, string email, string senha, string chamado)
        {
            // Arrange;
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            //var mapper = new Mock<IMapper>();
            var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            var criarUsuarioCommand = new Mock<ICriarUsuarioCommand>();
            var criarUsuarioCondicaoArbitrariaUseCase = new Mock<IObterUsuarioCondicaoArbitrariaUseCase>();
            var criarRefreshTokenUseCase = new Mock<ICriarRefreshTokenUseCase>();

            criarUsuarioCommand.Setup(x => x.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(new Usuario() { UsuarioId = 10, NomeCompleto = "Junior" }));

            var useCase = new CriarUsuarioUseCase(webHostEnvironment.Object, _map, jwtTokenGenerator.Object, criarUsuarioCommand.Object, criarUsuarioCondicaoArbitrariaUseCase.Object, criarRefreshTokenUseCase.Object);

            var input = new CriarUsuarioInput()
            {
                NomeCompleto = nomeCompleto,
                NomeUsuarioSistema = nomeUsuarioSistema,
                Email = email,
                Senha = senha,
                Chamado = chamado
            };

            // Act;
            var resp = await useCase.Execute(input);

            // Assert;
            Assert.True(resp.UsuarioId > 0);
            //Assert.Equal(resultadoEsperado, response.StatusCode);
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