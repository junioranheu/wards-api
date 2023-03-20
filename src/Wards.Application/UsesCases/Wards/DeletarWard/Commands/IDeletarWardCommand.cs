namespace Wards.Application.UsesCases.Wards.DeletarWard.Commands
{
    public interface IDeletarWardCommand
    {
        Task Execute(int id);
    }
}