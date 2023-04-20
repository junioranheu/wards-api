namespace Wards.Application.UseCases.Wards.DeletarWard.Commands
{
    public interface IDeletarWardCommand
    {
        Task<bool> Execute(int id);
    }
}