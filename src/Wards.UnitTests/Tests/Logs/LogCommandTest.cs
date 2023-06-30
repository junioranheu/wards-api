using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.Logs.CriarLog.Commands;
using Wards.Application.UseCases.Logs.Shared.Input;
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
            LogInput input = LogMock.CriarLogInput(tipoRequisicao, endpoint, parametros, descricao, statusResposta, usuarioId);

            // Act;
            await command.Execute(_map.Map<Log>(input));
            var db = await _context.Logs.FirstOrDefaultAsync(x => x.TipoRequisicao == tipoRequisicao && x.Endpoint == endpoint && x.UsuarioId == usuarioId);

            // Assert;
            Assert.Equal(db is not null, esperado);
        }
    }
}