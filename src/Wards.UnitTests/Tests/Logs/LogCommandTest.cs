using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Logs.ListarLog.Queries;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using Wards.UnitTests.Fixtures;
using Wards.UnitTests.Fixtures.Mocks;
using Xunit;

namespace Wards.UnitTests.Tests.Logs
{
    public sealed class LogCommandTest
    {
        private readonly WardsContext _context;
        private readonly IMapper _map;

        public LogCommandTest()
        {
            _context = Fixture.CriarContext();
            _map = Fixture.CriarMapper();
        }


        [Theory]
        [InlineData("POST", "TesteController/Teste", "Teste", "Isso é um teste válido", 200, 1, true)]
        [InlineData("", "", "", "Isso é um teste inválido", 0, 0, false)]
        public async Task Criar_ChecarResultadoEsperado(string tipoRequisicao, string endpoint, string parametros, string descricao, int statusResposta, int usuarioId, bool esperado)
        {
            // Arrange;
            var command = new CriarLogCommand(_context);
            LogInput input = LogMock.CriarInput(tipoRequisicao, endpoint, parametros, descricao, statusResposta, usuarioId);

            // Act;
            await command.Execute(_map.Map<Log>(input));
            var db = await _context.Logs.FirstOrDefaultAsync(x => x.TipoRequisicao == tipoRequisicao && x.Endpoint == endpoint && x.UsuarioId == usuarioId);

            // Assert;
            Assert.Equal(db is not null, esperado);
        }

        [Fact]
        public async Task Listar_ChecarResultadoEsperado()
        {
            // Arrange;
            var paginacao = new Mock<PaginacaoInput>();

            List<LogInput> listaInput = LogMock.CriarListaInput();
            await _context.Logs.AddRangeAsync(_map.Map<List<Log>>(listaInput));
            await _context.SaveChangesAsync();

            var query = new ListarLogQuery(_context);

            // Act;
            var resp = await query.Execute(paginacao.Object);

            // Assert;
            Assert.True(resp.Any());
        }
    }
}