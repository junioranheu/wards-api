using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Wards.BulkCopyCriarWard.Commands
{
    public interface IBulkCopyCriarWardCommand
    {
        Task Execute(List<Ward> input);
    }
}