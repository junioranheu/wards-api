using Microsoft.EntityFrameworkCore;
using Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands;
using Wards.Infrastructure.Data;
using Wards.UnitTests.Fixtures;
using Xunit;

namespace Wards.UnitTests.Tests.FeriadosDatas
{
    public sealed class FeriadoDataCommandTest
    {
        private readonly WardsContext _context;

        public FeriadoDataCommandTest()
        {
            _context = Fixture.CriarContext();
        }

        [Theory]
        [InlineData(new string[] { }, 1, false)]
        [InlineData(new string[] { "" }, 1, false)]
        [InlineData(new string[] { "01/01/0001" }, 1, false)]
        [InlineData(new string[] { "01/01/1997" }, 0, false)]
        [InlineData(new string[] { "06/02/1970", "17/01/1975", "25/03/1997", "19/12/1997" }, 1, true)]
        public async Task Criar_ChecarResultadoEsperado(string[] data, int? feriadoId, bool esperado)
        {
            // Arrange;
            var command = new CriarFeriadoDataCommand(_context);

            // Act;
            try
            {
                await command.Execute(data, feriadoId.GetValueOrDefault()); // Pegar um int?;
                var db = await _context.FeriadosDatas.FirstOrDefaultAsync(x => x.FeriadoId == feriadoId);

                // Assert;
                Assert.Equal(db is not null, esperado);
            }
            catch (Exception)
            {
                Assert.False(esperado);
            }
        }
    }
}