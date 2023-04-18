using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Wards.Application.UsesCases.Wards.CriarWard.Commands;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using Wards.UnitTests.Fixtures;
using Wards.UnitTests.Fixtures.Mocks;
using Xunit;

namespace Wards.UnitTests.Tests.Wards
{
    public sealed class WardCommandTest
    {
        private readonly WardsContext _context;
        private readonly IMapper _map;

        public WardCommandTest()
        {
            _context = Fixture.CriarContext();
            _map = Fixture.CriarMapper();
        }

        [Theory]
        [InlineData("a", "Para criar uma API você deverá bla bla bla", 1, true)]
        [InlineData("Como criar uma API", "a", 2, true)]
        [InlineData(null, "", 0, false)]
        [InlineData("", "", 0, false)]
        [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", 0, true)]
        [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", null, true)]
        [InlineData("Como criar uma API", "Para criar uma API você deverá bla bla bla", 10, true)]
        public async Task CriarWardCommand_ChecarResultadoEsperado(string titulo, string conteudo, int? usuarioId, bool esperado)
        {
            // Arrange;
            var logger = new Mock<ILogger<CriarWardCommand>>();

            var command = new CriarWardCommand(_context, logger.Object);

            WardInput input = WardMock.CriarWardInput(titulo, conteudo, usuarioId);

            // Act;
            var resp = await command.Execute(_map.Map<Ward>(input));
            var db = await _context.Wards.FirstOrDefaultAsync(x => x.Titulo == titulo);

            // Assert;
            Assert.Equal(db is not null, esperado);
        }
    }
}
