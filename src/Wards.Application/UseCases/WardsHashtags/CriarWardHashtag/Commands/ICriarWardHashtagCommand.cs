using Wards.Domain.Entities;

namespace Wards.Application.UseCases.WardsHashtags.CriarWardHashtag.Commands
{
    public interface ICriarWardHashtagCommand
    {
        Task Execute(List<WardHashtag> listaInput, int wardId);
    }
}