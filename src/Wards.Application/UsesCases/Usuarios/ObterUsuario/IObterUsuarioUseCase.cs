namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public interface IObterUsuarioUseCase
    {
        Task<Usuario> ExecuteAsync(int id);
        Task<int[]> GetListaIdUsuarioPerfil(string email);
    }
}