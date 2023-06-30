using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Wards.CriarWard.Commands;
using Wards.Application.UseCases.Wards.ListarWard.Queries;
using Wards.Application.UseCases.Wards.Shared.Input;
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
        public async Task Criar_ChecarResultadoEsperado(string titulo, string conteudo, int? usuarioId, bool esperado)
        {
            // Arrange;
            var command = new CriarWardCommand(_context);
            WardInput input = WardMock.CriarWardInput(titulo, conteudo, usuarioId);

            // Act;
            await command.Execute(_map.Map<Ward>(input));
            var db = await _context.Wards.FirstOrDefaultAsync(x => x.Titulo == titulo);

            // Assert;
            Assert.Equal(db is not null, esperado);
        }

        [Fact]
        public async Task Listar_ChecarResultadoEsperado()
        {
            // Arrange;
            var paginacao = new Mock<PaginacaoInput>();

            List<WardInput> listaInput = WardMock.CriarListaWardInput();
            await _context.Wards.AddRangeAsync(_map.Map<List<Ward>>(listaInput));
            await _context.SaveChangesAsync();

            var query = new ListarWardQuery(_context);

            // Act;
            var resp = await query.Execute(paginacao.Object);

            // Assert;
            Assert.True(resp.Count() > 0);
        }
    }
}