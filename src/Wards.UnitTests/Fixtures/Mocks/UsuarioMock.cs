using Wards.Application.UsesCases.Usuarios.Shared.Input;

namespace Wards.UnitTests.Fixtures.Mocks
{
    public static class UsuarioMock
    {
        public static CriarUsuarioInput CriarUsuarioInput(string nomeCompleto, string nomeUsuarioSistema, string email, string senha, string chamado)
        {
            CriarUsuarioInput usuario = new()
            {
                NomeCompleto = !string.IsNullOrEmpty(nomeCompleto) ? nomeCompleto : Guid.NewGuid().ToString(),
                NomeUsuarioSistema = !string.IsNullOrEmpty(nomeUsuarioSistema) ? nomeUsuarioSistema : Guid.NewGuid().ToString(),
                Email = !string.IsNullOrEmpty(email) ? email : Guid.NewGuid().ToString(),
                Senha = !string.IsNullOrEmpty(senha) ? senha : Guid.NewGuid().ToString(),
                Chamado = !string.IsNullOrEmpty(chamado) ? chamado : Guid.NewGuid().ToString()
            };

            return usuario;
        }
    }
}