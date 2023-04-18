﻿using Microsoft.Extensions.Logging;
using Moq;
using Wards.Application.UsesCases.Wards.CriarWard;
using Wards.Application.UsesCases.Wards.CriarWard.Commands;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Wards.Infrastructure.Data;
using Xunit;

namespace Wards.UnitTests.Tests.Wards
{
    public sealed class WardCommandTest
    {
        [Theory]
        // [InlineData("a", "Para criar uma API você deverá bla bla bla", 1, false)]
        // [InlineData("Como criar uma API", "a", 2, false)]
        // [InlineData("", "", 0, false)]
        // [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", 0, false)]
        // [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", null, false)]
        [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", 10, true)]
        public async Task CriarWardCommand_ChecarResultadoEsperado(string titulo, string conteudo, int? usuarioId, bool esperado)
        {
            // Arrange;
            var context = new Mock<WardsContext>();
            var logger = new Mock<ILogger<CriarWardCommand>>();

            criarWardCommand.Setup(x => x.Execute(It.IsAny<Ward>())).Returns(Task.FromResult(1));

            var useCase = new CriarWardCommand(context.Object, logger.Object);

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
