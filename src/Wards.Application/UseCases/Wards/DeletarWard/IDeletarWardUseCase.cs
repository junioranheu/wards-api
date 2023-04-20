namespace Wards.Application.UseCases.Wards.DeletarWard
{
    public interface IDeletarWardUseCase
    {
        Task<bool> Execute(int id);
    }
}