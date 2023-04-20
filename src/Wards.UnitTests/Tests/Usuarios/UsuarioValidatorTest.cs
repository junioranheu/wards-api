using FluentValidation.TestHelper;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Xunit;

namespace Wards.UnitTests.Tests.Usuarios
{
    public sealed class UsuarioValidatorTest
    {
        private readonly CriarUsuarioInputValidator _validator;

        public UsuarioValidatorTest()
        {
            _validator = new CriarUsuarioInputValidator();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Ju", false)]
        [InlineData("Junior", true)]
        [InlineData("Junior de Souza", true)]
        public void Validar_NomeCompleto(string nomeCompleto, bool esperado)
        {
            // Arrange;
            var model = new CriarUsuarioInput { NomeCompleto = nomeCompleto };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            if (esperado)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.NomeCompleto);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.NomeCompleto);
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("junior", false)]
        [InlineData("junior@", false)]
        [InlineData("junior@gmail", false)]
        [InlineData("junior@gmail.com", true)]
        [InlineData("mariana@hotmail.com", true)]
        [InlineData("pota@bol.com.br", true)]
        public void Validar_Email(string email, bool esperado)
        {
            // Arrange;
            var model = new CriarUsuarioInput { Email = email };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            if (esperado)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Email);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.Email);
            }
        }

        [Theory]
        [InlineData(null, "Junior", "junioranheu", "junioranheu@gmail.com", false)]
        [InlineData("", "Junior", "junioranheu", "junioranheu@gmail.com", false)]
        [InlineData("junior", "Junior", "junioranheu", "junioranheu@gmail.com", false)]
        [InlineData("junior30", "Junior", "junioranheu", "junioranheu@gmail.com", false)]
        [InlineData("Junior30@", "Junior", "junioranheu", "junioranheu@gmail.com", false)]
        [InlineData("Juninho30@", "Junior", "junioranheu", "junioranheu@gmail.com", true)]
        [InlineData("Junior30", "Junior", "junioranheu", "junioranheu@gmail.com", false)]
        [InlineData("Junin30", "Junior", "junioranheu", "junioranheu@gmail.com", true)]
        [InlineData("potaEuTeAmo26", "Junior", "junioranheu", "junioranheu@gmail.com", true)]
        public void Validar_Senha(string senha, string nomeCompleto, string nomeUsuarioSistema, string email, bool esperado)
        {
            // Arrange;
            var model = new CriarUsuarioInput { Senha = senha, NomeCompleto = nomeCompleto, NomeUsuarioSistema = nomeUsuarioSistema, Email = email };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            if (esperado)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Senha);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.Senha);
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("#1", true)]
        [InlineData("Chamado #22", true)]
        public void Validar_Chamado(string chamado, bool esperado)
        {
            // Arrange;
            var model = new CriarUsuarioInput { Chamado = chamado };

            // Act;
            var result = _validator.TestValidate(model);

            // Assert;
            if (esperado)
            {
                result.ShouldNotHaveValidationErrorFor(x => x.Chamado);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.Chamado);
            }
        }
    }
}