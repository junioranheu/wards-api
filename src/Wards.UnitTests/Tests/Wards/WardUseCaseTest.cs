using AutoMapper;
using Moq;
using Wards.Application.UsesCases.Wards.CriarWard;
using Wards.Application.UsesCases.Wards.CriarWard.Commands;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Wards.Domain.Entities;
using Wards.UnitTests.Utils;
using Xunit;

namespace Wards.UnitTests.Tests.Wards
{
    public sealed class WardUseCaseTest
    {
        private readonly IMapper _map;

        public WardUseCaseTest()
        {
            Factory f = new();
            _map = f.CriarMapper();
        }

        [Theory]
        // [InlineData("a", "Para criar uma API você deverá bla bla bla", 1, false)]
        // [InlineData("Como criar uma API", "a", 2, false)]
        // [InlineData("", "", 0, false)]
        // [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", 0, false)]
        // [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", null, false)]
        [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", 10, true)]
        public async Task CriarWardUseCase_ChecarResultadoEsperado(string titulo, string conteudo, int? usuarioId, bool esperado)
        {
            // Arrange;
            var criarWardCommand = new Mock<ICriarWardCommand>();
            criarWardCommand.Setup(x => x.Execute(It.IsAny<Ward>())).Returns(Task.FromResult(1));

            var useCase = new CriarWardUseCase(_map, criarWardCommand.Object);

            var input = new WardInput()
            {
                Titulo = titulo,
                Conteudo = conteudo,
                UsuarioId = usuarioId
            };

            // Act;
            var resp = await useCase.Execute(input);

            // Assert;
            Assert.Equal(resp > 0, esperado);
        }
    }
}