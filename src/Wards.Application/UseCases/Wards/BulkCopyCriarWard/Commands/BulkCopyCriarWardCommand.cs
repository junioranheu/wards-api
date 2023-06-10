using Wards.Domain.Entities;
using Wards.Infrastructure.Factory;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Wards.BulkCopyCriarWard.Commands
{
    public sealed class BulkCopyCriarWardCommand : IBulkCopyCriarWardCommand
    {
        private readonly IConnectionFactory _connectionFactory;

        public BulkCopyCriarWardCommand(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Execute(List<Ward> input)
        {
            await ConverterLINQQueryParaDataTableParaBulkInsert(input, "Wards", _connectionFactory.CreateDbSqlConnection());
        }
    }
}