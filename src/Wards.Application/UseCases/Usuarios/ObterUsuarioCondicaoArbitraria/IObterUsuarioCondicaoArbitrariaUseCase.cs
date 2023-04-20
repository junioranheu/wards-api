using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria
{
    public interface IObterUsuarioCondicaoArbitrariaUseCase
    {
        Task<(UsuarioOutput? usuario, string senha)> Execute(string login);
    }
}