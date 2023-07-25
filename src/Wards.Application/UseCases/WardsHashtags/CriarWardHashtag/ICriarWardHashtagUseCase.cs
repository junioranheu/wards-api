using Wards.Application.UseCases.WardsHashtags.Shared.Input;

namespace Wards.Application.UseCases.WardsHashtags.CriarWardHashtag
{
    public interface ICriarWardHashtagUseCase
    {
        Task Execute(List<WardHashtagInput> listaInput, int wardId);
    }
}