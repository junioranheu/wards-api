using AutoMapper;
using Wards.Application.UseCases.Wards.BulkCopyCriarWard.Commands;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.BulkCopyCriarWard
{
    public sealed class BulkCopyCriarWardUseCase : IBulkCopyCriarWardUseCase
    {
        private readonly IMapper _map;
        private readonly IBulkCopyCriarWardCommand _bulkCopyCommand;

        public BulkCopyCriarWardUseCase(IMapper map, IBulkCopyCriarWardCommand bulkCopyCommand)
        {
            _map = map;
            _bulkCopyCommand = bulkCopyCommand;
        }

        public async Task Execute(List<WardInput> input)
        {
            await _bulkCopyCommand.Execute(_map.Map<List<Ward>>(input));
        }
    }
}