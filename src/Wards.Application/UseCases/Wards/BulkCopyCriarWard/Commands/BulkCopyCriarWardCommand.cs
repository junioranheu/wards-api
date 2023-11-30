using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Bulk.BulkCopy;

namespace Wards.Application.UseCases.Wards.BulkCopyCriarWard.Commands
{
    public sealed class BulkCopyCriarWardCommand : IBulkCopyCriarWardCommand
    {
        private readonly WardsContext _context;

        public BulkCopyCriarWardCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task Execute(List<Ward> input)
        {
            await BulkInsert(input, _context, "Wards", isExceptionInPortuguese: true);
        }
    }
}