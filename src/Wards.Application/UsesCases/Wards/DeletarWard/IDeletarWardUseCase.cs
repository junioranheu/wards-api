namespace Wards.Application.UsesCases.Wards.DeletarWard
{
    public interface IDeletarWardUseCase
    {
        Task<bool> Execute(int id);
    }
}