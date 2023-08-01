namespace Wards.Application.UseCases.Hashtags.ListarHashtag
{
    public interface IListarHashtagUseCase
    {
        Task<List<string>> Execute();
    }
}