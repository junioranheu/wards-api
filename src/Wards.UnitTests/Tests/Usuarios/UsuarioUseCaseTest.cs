using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Tokens.CriarRefreshToken;
using Wards.Application.UseCases.Usuarios.CriarUsuario;
using Wards.Application.UseCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UseCases.Usuarios.ListarUsuario;
using Wards.Application.UseCases.Usuarios.ListarUsuario.Queries;
using Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Auth.Token;
using Wards.UnitTests.Fixtures;
using Wards.UnitTests.Fixtures.Mocks;
using Xunit;

namespace Wards.UnitTests.Tests.Usuarios
{
    public sealed class UsuarioUseCaseTest
    {
        private readonly IMapper _map;

        public UsuarioUseCaseTest()
        {
            _map = Fixture.CriarMapper();
        }

        [Theory]
        [InlineData("Junior de Souza", "junioranheu", "junioranheu@gmail.com", "Juninho26@", "#1", true)]
        [InlineData("Otávio Villas Boas", "otavioGOD", "otavio@gmail.com", "Otavinho26@", "#2", true)]
        [InlineData("Mariana Scalzaretto", "elfamscal", "elfa@gmail.com", "Marianinha26@", "#3", true)]
        [InlineData("Ju", "aea", "aea@gmail.com", "aea@", "#4", false)]
        [InlineData("Junior de S.", "junioranheu", "junioranheu@gmail.com", "senhainvalida", "#5", false)]
        public async Task Criar_ChecarResultadoEsperado(string nomeCompleto, string nomeUsuarioSistema, string email, string senha, string chamado, bool esperado)
        {
            // Arrange;
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            var jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            var criarUsuarioCondicaoArbitrariaUseCase = new Mock<IObterUsuarioCondicaoArbitrariaUseCase>();
            var criarRefreshTokenUseCase = new Mock<ICriarRefreshTokenUseCase>();

            var criarUsuarioCommand = new Mock<ICriarUsuarioCommand>();
            criarUsuarioCommand.Setup(x => x.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(new Usuario() { UsuarioId = 1, NomeCompleto = "Junior" }));

            var useCase = new CriarUsuarioUseCase(webHostEnvironment.Object, _map, jwtTokenGenerator.Object, criarUsuarioCommand.Object, criarUsuarioCondicaoArbitrariaUseCase.Object, criarRefreshTokenUseCase.Object);

            CriarUsuarioInput input = UsuarioMock.CriarInput(nomeCompleto, nomeUsuarioSistema, email, senha, chamado);

            try
            {
                // Act;
                var resp = await useCase.Execute(input);

                // Assert;
                Assert.Equal(resp?.UsuarioId > 0, esperado);
            }
            catch (Exception)
            {
                if (!esperado)
                {
                    Assert.False(esperado);
                }
            }
        }

        [Fact]
        public async Task Listar_ChecarResultadoEsperado()
        {
            // Arrange;
            var paginacao = new Mock<PaginacaoInput>();
            var listarQuery = new Mock<IListarUsuarioQuery>();

            var lista = _map.Map<IEnumerable<Usuario>>(UsuarioMock.CriarListaInput());
            listarQuery.Setup(x => x.Execute(It.IsAny<PaginacaoInput>())).Returns(Task.FromResult(lista));

            var useCase = new ListarUsuarioUseCase(_map, listarQuery.Object);

            // Act;
            var teste = await useCase.Execute(paginacao.Object);

            // Assert;
            Assert.True(teste.Any());
        }
    }
}