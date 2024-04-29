using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

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
            await Bulk.Helpers.BulkInsert(input, _context, "Wards", isExceptionInPortuguese: true);
        }
    }
}