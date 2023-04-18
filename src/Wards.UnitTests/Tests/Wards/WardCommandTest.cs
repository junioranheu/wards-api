using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Wards.Application.AutoMapper;
using Wards.Application.UsesCases.Wards.CriarWard.Commands;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using Xunit;

namespace Wards.UnitTests.Tests.Wards
{
    public sealed class WardCommandTest
    {
        private readonly IMapper _map;

        public WardCommandTest()
        {
            var mockMapper = new MapperConfiguration(x =>
            {
                x.AddProfile(new AutoMapperConfig());
            });

            _map = mockMapper.CreateMapper();
        }

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

            var command = new CriarWardCommand(context.Object, logger.Object);

            var input = new WardInput()
            {
                Titulo = titulo,
                Conteudo = conteudo,
                UsuarioId = usuarioId
            };

            // Act;
            var resp = await command.Execute(_map.Map<Ward>(input));

            // Assert;
            Assert.Equal(resp > 0, esperado);
        }
    }
}
