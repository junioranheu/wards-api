namespace Wards.Application.UsesCases.Wards.DeletarWard.Commands
{
    public interface IDeletarWardCommand
    {
        Task<bool> Execute(int id);
    }
}