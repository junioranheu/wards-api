using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria
{
    public interface IObterUsuarioCondicaoArbitrariaUseCase
    {
        Task<Usuario> Execute(string? email, string? nomeUsuarioSistema);
    }
}