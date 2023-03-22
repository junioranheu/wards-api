using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.UnitTests.Tests
{
    public class UsuariosTest
    {
        private readonly WardsContext _context;

        public UsuariosTest()
        {
            _context = new Configuration().CreateTestContext();
        }

        public static IEnumerable<object[]> Dados()
        {
            yield return new object[] { new Usuario { NomeCompleto = GerarStringAleatoria(5, false), NomeUsuarioSistema = GerarStringAleatoria(5, false), Email = GerarStringAleatoria(5, false), Senha = Criptografar(GerarStringAleatoria(5, false)), Chamado = GerarStringAleatoria(5, false) } };
            yield return new object[] { new Usuario { NomeCompleto = GerarStringAleatoria(5, false), NomeUsuarioSistema = GerarStringAleatoria(5, false), Email = GerarStringAleatoria(5, false), Senha = Criptografar(GerarStringAleatoria(5, false)), Chamado = GerarStringAleatoria(5, false) } };
            yield return new object[] { new Usuario { NomeCompleto = GerarStringAleatoria(5, false), NomeUsuarioSistema = GerarStringAleatoria(5, false), Email = GerarStringAleatoria(5, false), Senha = Criptografar(GerarStringAleatoria(5, false)), Chamado = GerarStringAleatoria(5, false) } };
        }

        //[Theory]
        //[MemberData(nameof(Dados))]
        //public async Task Criar_QuandoSucesso_RetornarOk(Usuario input)
        //{
        //    int resp = await new CriarUsuarioUseCase(new CriarUsuarioCommand(_context)).Execute(input);
        //    Assert.True(resp > 0);
        //}
    }
}