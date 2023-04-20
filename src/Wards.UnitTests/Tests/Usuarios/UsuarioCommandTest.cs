using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Wards.Application.UseCases.Shared.Models;
using Wards.Application.UseCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UseCases.Usuarios.ListarUsuario.Queries;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using Wards.UnitTests.Fixtures;
using Wards.UnitTests.Fixtures.Mocks;
using Xunit;

namespace Wards.UnitTests.Tests.Usuarios
{
    public sealed class UsuarioCommandTest
    {
        private readonly WardsContext _context;
        private readonly IMapper _map;

        public UsuarioCommandTest()
        {
            _context = Fixture.CriarContext();
            _map = Fixture.CriarMapper();
        }

        [Theory]
        [InlineData("Junior de Souza", "junioranheu", "junioranheu@gmail.com", "Juninho26@", "#1", true)]
        [InlineData("Otávio Villas Boas", "otavioGOD", "otavio@gmail.com", "Otavinho26@", "#2", true)]
        [InlineData("Mariana Scalzaretto", "elfamscal", "elfa@gmail.com", "Marianinha26@", "#3", true)]
        [InlineData("Ju", "aea", "aea@gmail.com", "aea@", "#4", true)]
        [InlineData("Junior de S.", "junioranheu", "junioranheu@gmail.com", "tmr-pes-weon", "#5", true)]
        [InlineData("", "", "", "", "", false)]
        public async Task CriarUsuarioCommand_ChecarResultadoEsperado(string nomeCompleto, string nomeUsuarioSistema, string email, string senha, string chamado, bool esperado)
        {
            // Arrange;
            var command = new CriarUsuarioCommand(_context);
            CriarUsuarioInput input = UsuarioMock.CriarUsuarioInput(nomeCompleto, nomeUsuarioSistema, email, senha, chamado);

            // Act;
            await command.Execute(_map.Map<Usuario>(input));
            var db = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == email);

            // Assert;
            Assert.Equal(db is not null, esperado);
        }

        [Fact]
        public async Task ListarUsuarioQuery_ChecarResultadoEsperado()
        {
            // Arrange;
            var paginacao = new Mock<PaginacaoInput>();

            List<CriarUsuarioInput> listaInput = UsuarioMock.CriarListaUsuarioInput();
            await _context.Usuarios.AddRangeAsync(_map.Map<List<Usuario>>(listaInput));
            await _context.SaveChangesAsync();

            var query = new ListarUsuarioQuery(_context);

            // Act;
            var resp = await query.Execute(paginacao.Object);

            // Assert;
            Assert.True(resp is not null);
        }
    }
}