namespace Wards.Application.UsesCases.Wards.CriarWard
{
    public interface ICriarWardUseCase
    {
        Task<int> Execute(WardInput input);
    }
}