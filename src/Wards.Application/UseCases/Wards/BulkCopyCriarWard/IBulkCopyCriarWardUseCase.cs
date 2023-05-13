using Wards.Application.UseCases.Wards.Shared.Input;

namespace Wards.Application.UseCases.Wards.BulkCopyCriarWard
{
    public interface IBulkCopyCriarWardUseCase
    {
        Task Execute(List<WardInput> input);
    }
}