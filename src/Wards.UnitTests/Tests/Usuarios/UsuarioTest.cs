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
            Usuario u = new() { UsuarioId = 1 };
            command.Setup(x => x.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(u));

            var sut = new CriarUsuarioUseCase(map.Object, command.Object);
            var input = new UsuarioInput() { UsuarioId = 1 };

            // Act;
            var resp = await sut.Execute(input);

            // Assert;
            Assert.True(resp.UsuarioId > 0);
        }

        [Fact]
        public async Task CriarUsuarioUseCase_DeveRetornar0_QuandoFalhaNoBanco()
        {
            // Arrange;
            var map = new Mock<IMapper>();
            var command = new Mock<ICriarUsuarioCommand>();
            Usuario u = new() { UsuarioId = 0 };
            command.Setup(x => x.Execute(It.IsAny<Usuario>())).Returns(Task.FromResult(u));

            var sut = new CriarUsuarioUseCase(map.Object, command.Object);
            var input = new UsuarioInput() { UsuarioId = 0 };

            // Act;
            var resp = await sut.Execute(input);

            // Assert;
            Assert.True(resp.UsuarioId < 1);
        }
    }
}