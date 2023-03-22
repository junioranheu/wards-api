using AutoMapper;
using Moq;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Xunit;

namespace Wards.UnitTests.Tests.Usuarios
{
    public sealed class UsuarioTest
    {
        [Fact]
        public async Task CriarUsuarioUseCase_DeveRetornar1_QuandoParametrosValidosEBanco()
        {
            // Arrange;
            var map = new Mock<IMapper>();
            var command = new Mock<ICriarUsuarioCommand>();
            command.Setup(c => c.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(1));

            var sut = new CriarUsuarioUseCase(map.Object, command.Object);
            var input = new UsuarioInput() { };

            // Act;
            var resp = await sut.Execute(input);

            // Assert;
            Assert.Equal(1, resp);
        }

        [Fact]
        public async Task CriarUsuarioUseCase_DeveRetornar0_QuandoFalhaNoBanco()
        {
            // Arrange;
            var map = new Mock<IMapper>();
            var command = new Mock<ICriarUsuarioCommand>();
            command.Setup(c => c.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(0));

            var sut = new CriarUsuarioUseCase(map.Object, command.Object);
            var input = new UsuarioInput() { };

            // Act;
            var resp = await sut.Execute(input);

            // Assert;
            Assert.Equal(0, resp);
        }
    }
}