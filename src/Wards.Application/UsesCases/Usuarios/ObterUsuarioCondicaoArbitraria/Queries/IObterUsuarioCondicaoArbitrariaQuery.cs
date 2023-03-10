using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries
{
    public interface IObterUsuarioCondicaoArbitrariaQuery
    {
        Task<(Usuario, string)> Execute(string? email, string? nomeUsuarioSistema);
    }
}